using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace QA.DotNetCore.Caching.Distributed
{
    /// <summary>
    /// Distributed cache using Redis.
    /// <para>Uses <c>StackExchange.Redis</c> as the Redis client.</para>
    /// </summary>
    public partial class RedisCache : IDistributedTaggedCache
    {
        /// <summary>
        /// Sets tag new expiry only if its exceeds current expiry.
        /// <list type="table">
        ///     <item>KEYS[1] - Tag key</item>
        ///     <item>ARGV[1] - New tag expiry in seconds</item>
        /// </list>
        /// </summary>
        /// 
        /// <remarks>
        /// Tags can be linked to multiple keys. That is why
        /// a key should be allowed only to incread tag expiry
        /// (if it has been specified previously by another key).
        /// Otherwise it can lead to a situation when a tag is expired
        /// before some of its keys.
        /// </remarks>
        private const string ExtendKeyExpiryScript = @"
            local key = KEYS[1]
            local key_expiry = tonumber(redis.call('TTL', key))
            local new_expiry = tonumber(ARGV[1])

            if new_expiry > key_expiry then
              redis.call('EXPIRE', key, new_expiry)
            end
            ";

        /// <summary>
        /// Invalidates tag, metadata and associated keys.
        /// <list type="table">
        ///     <item>KEYS[1] - Tag key to invalidate</item>
        ///     <item>ARGV[1] - Lock key (for optimistic lock in GetOrAdd)</item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Also increments tag lock key to invalidate operation to save
        /// (already) stale cache that are in-progress.
        /// </remarks>
        private const string InvalidateTagScript = @"
            local tag = KEYS[1]
            local lock = ARGV[1]

            -- Increments lock without overflow.
            local function safe_increment(lock)
              if type(redis.pcall('INCR', lock)) ~= 'number' then
                -- On overflow error
                redis.call('SET', lock, '1')
              end
            end

            safe_increment(lock)
            redis.call('EXPIRE', lock, '1200')

            local keys = redis.call('SMEMBERS', tag)
            redis.call('DEL', tag, 'pack:' .. tag, unpack(keys))
            ";

        /// <summary>
        /// Compacts tag keys by checking their existance.
        /// <list type="table">
        ///     <item>KEYS[1] - Tag key to compact</item>
        /// </list>
        /// </summary>
        private const string CompactTagKeysScript = @"
            local tag = KEYS[1]
            local keys = redis.call('SMEMBERS', tag)
            for i=1,#keys do
              if redis.call('EXISTS', keys[i]) == 0 then
                redis.call('SREM', tag, keys[i])
              end
            end
            ";

        private static readonly Version LeastSupportedServerVersion = new Version(4, 0, 0);

        private readonly RedisCacheSettings _options;
        private readonly CacheKeyFactory _keyFactory;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        private volatile IConnectionMultiplexer _connection;
        private IDatabase _cache;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of <see cref="RedisCache"/>.
        /// </summary>
        /// <param name="optionsAccessor">The configuration options.</param>
        public RedisCache(IOptions<RedisCacheSettings> optionsAccessor)
        {
            if (optionsAccessor is null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _options = optionsAccessor.Value;

            _keyFactory = new CacheKeyFactory(_options.InstanceName);
        }

        public string GetClientId(CancellationToken token = default)
        {
            Connect(token);

            const int maxAttempts = 20;
            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                string guid = Guid.NewGuid().ToString();
                if (_cache.StringSet(guid, string.Empty, when: When.NotExists))
                {
                    return guid;
                }
            }

            throw new InvalidOperationException($"Unable to generate unique client id.");
        }

        public bool IsExist(string key, CancellationToken token = default)
        {
            RedisKey dataKey = _keyFactory.CreateKey(key).GetRedisKey();

            Connect(token);

            return _cache.KeyExists(dataKey);
        }

        public byte[] Get(string key, CancellationToken token = default)
        {
            CacheKey dataKey = _keyFactory.CreateKey(key);

            Connect(token);

            _ = TryGet(dataKey, out byte[] cachedData);

            return cachedData;
        }

        public void Invalidate(string key, CancellationToken token = default)
        {
            RedisKey dataKey = _keyFactory.CreateKey(key).GetRedisKey();

            Connect(token);

            _ = _cache.KeyDelete(dataKey);
        }

        public void InvalidateTag(string tag, CancellationToken token = default)
        {
            CacheKey tagKey = _keyFactory.CreateTag(tag);
            RedisValue lockKey = _keyFactory.CreateLock(tagKey).GetRedisValue();

            Connect(token);

            _ = _cache.ScriptEvaluate(
                InvalidateTagScript,
                new[] { tagKey.GetRedisKey() },
                new[] { lockKey });
        }

        public byte[] GetOrAdd(
            string key,
            string[] tags,
            TimeSpan expiry,
            Func<MemoryStream> dataStreamFactory,
            CancellationToken token = default)
        {
            if (tags is null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            if (dataStreamFactory is null)
            {
                throw new ArgumentNullException(nameof(dataStreamFactory));
            }

            CacheKey dataKey = _keyFactory.CreateKey(key);
            IEnumerable<CacheKey> tagKeys = _keyFactory.CreateTags(tags).ToList();

            Connect(token);

            if (TryGet(dataKey, out byte[] cachedData))
            {
                return cachedData;
            }

            IEnumerable<Condition> invalidationsState = tagKeys.Select(WatchTagInvalidation).ToList();

            var data = RedisValue.CreateFrom(dataStreamFactory());

            _ = TrySet(dataKey, tagKeys, expiry, data, invalidationsState);

            return data;
        }

        public void Set(
            string key,
            IEnumerable<string> tags,
            TimeSpan expiry,
            MemoryStream dataStream,
            CancellationToken token = default)
        {
            if (tags is null)
            {
                throw new ArgumentNullException(nameof(tags));
            }

            CacheKey dataKey = _keyFactory.CreateKey(key);
            IEnumerable<CacheKey> tagKeys = _keyFactory.CreateTags(tags).ToList();

            Connect(token);

            _ = TrySet(dataKey, tagKeys, expiry, RedisValue.CreateFrom(dataStream));
        }

        private bool TrySet(
            CacheKey key,
            IEnumerable<CacheKey> tags,
            TimeSpan expiry,
            RedisValue data,
            IEnumerable<Condition> conditions = null)
        {
            ITransaction transaction = CreateSetCacheTransaction(key, tags, expiry, data, conditions);
            bool result = transaction.Execute();

            var tagExpiry = GetTagExpiry(expiry);
            foreach (var tag in tags)
            {
                CompactTag(tag, tagExpiry);
            }

            return result;
        }

        private ITransaction CreateSetCacheTransaction(
            CacheKey key,
            IEnumerable<CacheKey> tags,
            TimeSpan expiry,
            RedisValue data,
            IEnumerable<Condition> conditions)
        {
            ITransaction transaction = _cache.CreateTransaction();

            foreach (var condition in conditions ?? Enumerable.Empty<Condition>())
            {
                _ = transaction.AddCondition(condition);
            }

            var dataKey = key.GetRedisKey();
            var tagExpiry = GetTagExpiry(expiry);

            _ = transaction.StringSetAsync(dataKey, data, expiry);

            foreach (var tag in tags)
            {
                RedisKey tagKey = tag.GetRedisKey();
                RedisKey packTagKey = _keyFactory.CreatePack(tag).GetRedisKey();
                RedisValue tagExpiryValue = new RedisValue(tagExpiry.TotalSeconds.ToString());

                _ = transaction.SetAddAsync(tagKey, key.GetRedisValue());
                _ = transaction.ScriptEvaluateAsync(ExtendKeyExpiryScript, new[] { tagKey }, new[] { tagExpiryValue });
                _ = transaction.ScriptEvaluateAsync(ExtendKeyExpiryScript, new[] { packTagKey }, new[] { tagExpiryValue });
            }

            return transaction;
        }

        private bool TryGet(CacheKey key, out byte[] data)
        {
            RedisKey dataKey = key.GetRedisKey();
            RedisValue cachedData = _cache.StringGet(dataKey);

            if (!cachedData.HasValue)
            {
                data = default;
                return false;
            }

            // TODO: Handle exception
            data = cachedData;
            return true;
        }

        private void CompactTag(CacheKey tag, TimeSpan expiry)
        {
            RedisKey packKey = _keyFactory.CreatePack(tag.Key).GetRedisKey();
            RedisValue compactAttempts = _cache.StringGet(packKey);

            ITransaction transaction = CreateTagCompactingTransaction(tag, expiry, compactAttempts);
            _ = transaction.Execute();
        }

        private ITransaction CreateTagCompactingTransaction(CacheKey tag, TimeSpan expiry, RedisValue compactAttempts)
        {
            bool isPreviouslyCompacted = compactAttempts.HasValue;
            RedisKey packKey = _keyFactory.CreatePack(tag.Key).GetRedisKey();

            ITransaction transaction = _cache.CreateTransaction();

            _ = transaction.AddCondition(
                Condition.SetLengthGreaterThan(tag.GetRedisKey(), _options.CompactTagSizeThreshold));

            if (isPreviouslyCompacted && int.Parse(compactAttempts) < _options.CompactTagFrequency)
            {
                _ = transaction.StringIncrementAsync(packKey);
                _ = transaction.KeyExpireAsync(packKey, expiry);
            }
            else
            {
                _ = transaction.AddCondition(Condition.StringEqual(packKey, compactAttempts));
                _ = transaction.StringSetAsync(packKey, 0, expiry);
                _ = transaction.ScriptEvaluateAsync(CompactTagKeysScript, new[] { tag.GetRedisKey() });
            }

            return transaction;
        }

        private TimeSpan GetTagExpiry(TimeSpan keyExpiry) =>
            keyExpiry + _options.TagExpirationOffset;

        private Condition WatchTagInvalidation(CacheKey tagKey)
        {
            var lockKey = _keyFactory.CreateLock(tagKey).GetRedisKey();

            RedisValue lockValue = _cache.StringGet(lockKey);

            return lockValue.HasValue
                ? Condition.StringEqual(lockKey, lockValue)
                : Condition.KeyNotExists(lockKey);
        }

        private void Connect(CancellationToken token)
        {
            CheckDisposed();
            token.ThrowIfCancellationRequested();

            if (_cache != null)
            {
                return;
            }

            _connectionLock.Wait(token);
            try
            {
                if (_cache != null)
                {
                    return;
                }

                _connection = ConnectionMultiplexer.Connect(_options.Configuration);
                ValidateServerFeatures();
                _cache = _connection.GetDatabase();
            }
            finally
            {
                _ = _connectionLock.Release();
            }
        }

        private void ValidateServerFeatures()
        {
            _ = _connection ?? throw new InvalidOperationException($"{nameof(_connection)} cannot be null.");

            foreach (var endPoint in _connection.GetEndPoints())
            {
                if (_connection.GetServer(endPoint).Version < LeastSupportedServerVersion)
                {
                    throw new InvalidOperationException(
                        $"Redis version of endpoint {endPoint} ({_connection.GetServer(endPoint).Version}) " +
                        $"is not supported. Should be >={LeastSupportedServerVersion}.");
                }
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _connection?.Close();
        }

        private void CheckDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
