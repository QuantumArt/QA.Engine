using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching.Helpers.Operations;
using QA.DotNetCore.Caching.Interfaces;
using StackExchange.Redis;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
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

        private const string GetMultipleKeysExistance = @"
            local existList = {}
            for i=1,#KEYS do
              local isExist = redis.call('EXISTS', KEYS[i])
              table.insert(existList, isExist)
            end
            return existList
            ";

        private static readonly Version LeastSupportedServerVersion = new Version(4, 0, 0);
        private static readonly IReadOnlyList<byte[]> s_emptyResult = new List<byte[]>(0);

        private readonly RedisCacheSettings _options;
        private readonly CacheKeyFactory _keyFactory;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);
        private readonly ILogger<RedisCache> _logger;
        private volatile IConnectionMultiplexer _connection;
        private IDatabase _cache;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of <see cref="RedisCache"/>.
        /// </summary>
        /// <param name="optionsAccessor">The configuration options.</param>
        public RedisCache(IOptions<RedisCacheSettings> optionsAccessor, ILogger<RedisCache> logger)
        {
            if (optionsAccessor is null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _options = optionsAccessor.Value;
            _logger = logger;

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

        public IEnumerable<bool> Exist(IEnumerable<string> keys, CancellationToken token = default)
        {
            if (keys is null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            if (!keys.Any())
            {
                return Enumerable.Empty<bool>();
            }

            RedisKey[] dataKeys = keys
                .Select(key => _keyFactory.CreateKey(key).GetRedisKey())
                .ToArray();

            Connect(token);

            if (dataKeys.Length == 1)
            {
                return new[] { _cache.KeyExists(dataKeys[0]) };
            }

            return (bool[])_cache.ScriptEvaluate(GetMultipleKeysExistance, dataKeys);
        }

        public IEnumerable<byte[]> Get(IEnumerable<string> keys, CancellationToken token = default)
        {
            if (keys is null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            if (!keys.Any())
            {
                return Enumerable.Empty<byte[]>();
            }

            var dataKeys = keys
                .Select(_keyFactory.CreateKey)
                .ToArray();

            Connect(token);

            return InnerGet(dataKeys);
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

        public IReadOnlyList<byte[]> GetOrAdd<TId>(
            CacheInfo<TId>[] cacheInfos,
            DataValuesFactoryDelegate<TId, MemoryStream> dataStreamsFactory,
            CancellationToken token = default)
        {
            if (cacheInfos is null)
            {
                throw new ArgumentNullException(nameof(cacheInfos));
            }

            if (dataStreamsFactory is null)
            {
                throw new ArgumentNullException(nameof(dataStreamsFactory));
            }

            if (cacheInfos.Length <= 0)
            {
                return s_emptyResult;
            }

            Connect(token);

            return new OperationsChain<CacheInfo<TId>, byte[]>()
                .AddOperation(GetCachedValues, isFinal: result => result != null)
                .AddOperation(infos => ObtainAndCacheRealValues(infos, dataStreamsFactory))
                .Execute(cacheInfos)
                .ToList();
        }

        private IEnumerable<byte[]> GetCachedValues<TId>(IEnumerable<CacheInfo<TId>> infos)
        {
            CacheKey[] cacheKeys = infos
                .Select(info => _keyFactory.CreateKey(info.Key))
                .ToArray();

            return InnerGet(cacheKeys);
        }

        private IEnumerable<byte[]> ObtainAndCacheRealValues<TId>(
            CacheInfo<TId>[] infos,
            DataValuesFactoryDelegate<TId, MemoryStream> dataStreamsFactory)
        {
            Debug.Assert(infos != null);

            var tagKeysGroups = new CacheKey[infos.Length][];
            var allTagKeys = new List<CacheKey>(infos.Length);

            for (int infoIndex = 0; infoIndex < infos.Length; infoIndex++)
            {
                tagKeysGroups[infoIndex] = new CacheKey[infos[infoIndex].Tags.Length];
                for (int tagIndex = 0; tagIndex < infos[infoIndex].Tags.Length; tagIndex++)
                {
                    tagKeysGroups[infoIndex][tagIndex] = _keyFactory.CreateTag(infos[infoIndex].Tags[tagIndex]);
                }

                allTagKeys.AddRange(tagKeysGroups[infoIndex]);
            }

            // Obtain tags' invalidation versions for optimistic lock.
            Condition[] conditions = WatchTagInvalidations(allTagKeys);

            var conditionGroups = new IEnumerable<Condition>[infos.Length];
            for (int lastOffset = 0, i = 0; i < infos.Length; i++)
            {
                conditionGroups[i] = conditions
                    .Skip(lastOffset)
                    .Take(infos[i].Tags.Length);

                lastOffset += infos[i].Tags.Length;
            }

            // Get real data for missing cache.
            var dataStreams = dataStreamsFactory(infos);

            int recievedIndex;
            using IEnumerator<MemoryStream> dataStreamsEnumerator = dataStreams.GetEnumerator();
            // Set missing cache with real data.
            for (recievedIndex = 0; dataStreamsEnumerator.MoveNext(); recievedIndex++)
            {
                var dataStream = dataStreamsEnumerator.Current;
                var data = RedisValue.CreateFrom(dataStream);

                yield return data;

                var info = infos[recievedIndex];
                var expiry = info.Expiration;
                var cacheKey = _keyFactory.CreateKey(info.Key);
                var tagKeys = tagKeysGroups[recievedIndex];
                var invalidationsState = conditionGroups[recievedIndex];

                _ = TrySet(cacheKey, tagKeys, expiry, data, invalidationsState);
            }

            CheckResultLength(recievedIndex, infos.Length, nameof(dataStreamsFactory));
        }

        private void CheckResultLength(int actual, int expected, string parameterName)
        {
            if (actual != expected)
            {
                throw new InvalidOperationException(
                    $"Factory {parameterName} should return the same number of elements it recieved " +
                    $"(expected: {expected}, returned: {actual})");
            }
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
            CacheKey[] tagKeys = _keyFactory.CreateTags(tags).ToArray();

            Connect(token);

            _ = TrySet(dataKey, tagKeys, expiry, RedisValue.CreateFrom(dataStream));
        }

        private bool TrySet(
            CacheKey key,
            IReadOnlyList<CacheKey> tags,
            TimeSpan expiry,
            RedisValue data,
            IEnumerable<Condition> conditions = null)
        {
            try
            {
                ITransaction transaction = CreateSetCacheTransaction(key, tags, expiry, data, conditions);
                bool isExecuted = transaction.Execute();

                if (isExecuted)
                {
                    var tagExpiry = GetTagExpiry(expiry);
                    CompactTags(tags, tagExpiry);
                }

                return isExecuted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to set cache with the key {CacheKey}", key);
                return false;
            }
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

        private IEnumerable<byte[]> InnerGet(in IReadOnlyList<CacheKey> keys)
        {
            RedisKey[] redisKeys = new RedisKey[keys.Count];
            try
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    redisKeys[i] = keys[i].GetRedisKey();
                }

                return _cache.StringGet(redisKeys)
                    .Select(result => result.HasValue ? (byte[])result : null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get multiple caches ('{0}').", redisKeys);
                return Enumerable.Repeat(Array.Empty<byte>(), keys.Count);
            }
        }

        private void CompactTags(IReadOnlyList<CacheKey> tags, TimeSpan expiry)
        {
            RedisKey[] packKeys = tags.Select(tag => _keyFactory.CreatePack(tag.Key).GetRedisKey()).ToArray();
            RedisValue[] compactAttempts = _cache.StringGet(packKeys);

            ITransaction transaction = CreateTagCompactingTransaction(tags, expiry, packKeys, compactAttempts);
            _ = transaction.Execute();
        }

        private ITransaction CreateTagCompactingTransaction(IReadOnlyList<CacheKey> tags, TimeSpan expiry, RedisKey[] packKeys, RedisValue[] compactAttempts)
        {
            if (tags.Count != packKeys.Length)
            {
                throw new ArgumentException(
                    $"Invalid size of pack keys (expected {tags.Count}, but was {packKeys.Length}).",
                    nameof(packKeys));
            }

            if (tags.Count != compactAttempts.Length)
            {
                throw new ArgumentException(
                    $"Invalid size of compact attempts collection (expected {tags.Count}, but was {compactAttempts.Length}).",
                    nameof(packKeys));
            }

            ITransaction transaction = _cache.CreateTransaction();

            for (int i = 0; i < tags.Count; i++)
            {
                var tag = tags[i];
                var packKey = packKeys[i];
                var compactAttempt = compactAttempts[i];

                bool isPreviouslyCompacted = compactAttempt.HasValue;

                _ = transaction.AddCondition(
                    Condition.SetLengthGreaterThan(tag.GetRedisKey(), _options.CompactTagSizeThreshold));

                if (isPreviouslyCompacted && int.Parse(compactAttempt) < _options.CompactTagFrequency)
                {
                    _ = transaction.StringIncrementAsync(packKey);
                    _ = transaction.KeyExpireAsync(packKey, expiry);
                }
                else
                {
                    _ = transaction.AddCondition(Condition.StringEqual(packKey, compactAttempt));
                    _ = transaction.StringSetAsync(packKey, 0, expiry);
                    _ = transaction.ScriptEvaluateAsync(CompactTagKeysScript, new[] { tag.GetRedisKey() });
                }
            }

            return transaction;
        }

        private TimeSpan GetTagExpiry(TimeSpan keyExpiry) =>
            keyExpiry + _options.TagExpirationOffset;

        private Condition[] WatchTagInvalidations(IReadOnlyList<CacheKey> tagKeys)
        {
            RedisKey[] lockKeys = new RedisKey[tagKeys.Count];
            for (int i = 0; i < tagKeys.Count; i++)
            {
                lockKeys[i] = _keyFactory.CreateLock(tagKeys[i]).GetRedisKey();
            }

            RedisValue[] lockValues = _cache.StringGet(lockKeys);

            Condition[] conditions = new Condition[lockValues.Length];
            for (int i = 0; i < lockValues.Length; i++)
            {
                var lockKey = lockKeys[i];
                var lockValue = lockValues[i];

                conditions[i] = lockValue.HasValue
                    ? Condition.StringEqual(lockKey, lockValue)
                    : Condition.KeyNotExists(lockKey);
            }

            return conditions;
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
