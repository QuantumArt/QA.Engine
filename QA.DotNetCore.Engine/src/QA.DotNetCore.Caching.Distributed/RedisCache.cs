using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching.Helpers.Operations;
using QA.DotNetCore.Caching.Interfaces;
using RedLockNet;
using StackExchange.Redis;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        ///     <item>ARGV[1] - New tag expiry in milliseconds</item>
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
            local key_expiry = tonumber(redis.call('PTTL', key))
            local new_expiry = tonumber(ARGV[1])

            if new_expiry > key_expiry then
              redis.call('PEXPIRE', key, new_expiry)
            end
            ";

        /// <summary>
        /// Invalidates tag, metadata and associated keys.
        /// <list type="table">
        ///     <item>KEYS[1] - Tag key to invalidate</item>
        ///     <item>ARGV[1] - Lock key (for optimistic lock in GetOrAdd)</item>
        ///     <item>ARGV[2] - Pack key prefix (to track pack count of tags)</item>
        ///     <item>ARGV[3] - Deprecated key time to live (to deprecated keys related to tag)</item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Also increments tag lock key to invalidate operation to save
        /// (already) stale cache that are in-progress.
        /// </remarks>
        private const string InvalidateTagScript = @"
            local tag = KEYS[1]
            local lock = ARGV[1]
            local packPrefix = ARGV[2]
            local deprecatedTtl = tonumber(ARGV[3])

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
            redis.call('DEL', tag, packPrefix .. tag)

            for i=1,#keys do
              redis.call('PEXPIRE', keys[i], deprecatedTtl)
            end
            ";

        /// <summary>
        /// Compacts tag keys by checking their existance.
        /// <list type="table">
        ///     <item>KEYS[1] - Tag key to compact</item>
        ///     <item>ARGV[1] - Deprecated key time to live (to deprecated keys related to tag)</item>
        /// </list>
        /// </summary>
        private const string CompactTagKeysScript = @"
            local tag = KEYS[1]
            local deprecatedTtl = tonumber(ARGV[1])
            local keys = redis.call('SMEMBERS', tag)
            
            for i=1,#keys do
              local ttl = redis.call('PTTL', keys[i])
              if ttl ~= -1 and ttl <= deprecatedTtl then
                redis.call('SREM', tag, keys[i])
              end
            end
            ";

        private static readonly Version LeastSupportedServerVersion = new(4, 0, 0);
        private static readonly IReadOnlyList<byte[]> s_emptyResult = new List<byte[]>(0);

        private static void FixupAndValidateExpiration(ref TimeSpan expiration) =>
            CacheInfo<object>.FixupAndValidateExpiration(ref expiration);

        private readonly RedisCacheSettings _options;
        private readonly CacheKeyFactory _keyFactory;
        private readonly IDistributedLockFactory _distributedLockFactory;
        private readonly ILogger<RedisCache> _logger;
        private readonly SemaphoreSlim _connectionLock = new(initialCount: 1, maxCount: 1);

        private volatile IConnectionMultiplexer _connection;
        private IDatabase _cache;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of <see cref="RedisCache"/>.
        /// </summary>
        /// <param name="optionsAccessor">The configuration options.</param>
        public RedisCache(
            IDistributedLockFactory distributedLockFactory,
            IOptions<RedisCacheSettings> optionsAccessor,
            ILogger<RedisCache> logger)
        {
            if (optionsAccessor is null)
            {
                throw new ArgumentNullException(nameof(optionsAccessor));
            }

            _distributedLockFactory = distributedLockFactory;
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
                    _logger.LogInformation("Selected new client id {ClientId}.", guid);
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

            var keyTtlTasks = dataKeys
                .Select(key => _cache.KeyTimeToLiveAsync(key))
                .ToList();

            TimeSpan?[] keyTtls = _cache.Wait(Task.WhenAll(keyTtlTasks));

            _logger.LogTrace("Keys {Keys} have ttls: {KeyTtls}", keys, keyTtls);

            bool[] existFlags = keyTtls
                .Select(ttl => ttl > _options.DeprecatedCacheTimeToLive)
                .ToArray();

            _logger.LogInformation("Keys {Keys} exist {ExistFlags}", keys, existFlags);

            return existFlags;
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

            return GetWithStates(dataKeys)
                .Select(result => result.State == KeyState.Exist ? result.Value : null);
        }

        public void Invalidate(string key, CancellationToken token = default)
        {
            RedisKey dataKey = _keyFactory.CreateKey(key).GetRedisKey();

            Connect(token);

            _ = _cache.KeyExpire(dataKey, _options.DeprecatedCacheTimeToLive);
        }

        public void InvalidateTag(string tag, CancellationToken token = default)
        {
            CacheKey tagKey = _keyFactory.CreateTag(tag);
            RedisValue lockKey = _keyFactory.CreateLock(tagKey).GetRedisValue();
            RedisValue packPrefix = new(_keyFactory.GetPackPrefix());
            RedisValue deprecatedTtl = (RedisValue)(long)_options.DeprecatedCacheTimeToLive.TotalMilliseconds;

            Connect(token);

            _ = _cache.ScriptEvaluate(
                InvalidateTagScript,
                new[] { tagKey.GetRedisKey() },
                new[] { lockKey, packPrefix, deprecatedTtl });
        }

        public IReadOnlyList<byte[]> GetOrAdd<TId>(
            CacheInfo<TId>[] cacheInfos,
            DataValuesFactoryDelegate<TId, MemoryStream> dataStreamsFactory,
            TimeSpan lockEnterWaitTimeout,
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

            IDisposable locker = null;
            try
            {
                return new OperationsChain<CacheInfo<TId>, CachedValue>(_logger)
                    .AddOperation(GetCachedValues, isFinal: result => result.State == KeyState.Exist)
                    .AddOperation((infos, context) => LockOrGetDeprecatedCache(infos, context, lockEnterWaitTimeout, out locker))
                    .AddOperation(GetCachedValues, isFinal: result => result.State == KeyState.Exist)
                    .AddOperation(infos => ObtainAndCacheRealValues(infos, dataStreamsFactory))
                    .Execute(cacheInfos)
                    .Select(result => result.Value)
                    .ToList();
            }
            finally
            {
                locker?.Dispose();
            }
        }

        private IEnumerable<OperationResult<CachedValue>> LockOrGetDeprecatedCache<TId>(
            CacheInfo<TId>[] infos,
            OperationContext<CachedValue> context,
            TimeSpan lockEnterWaitTimeout,
            out IDisposable locker)
        {
            var cacheKeys = infos.Select(info => info.Key).ToList();

            _logger.LogInformation("Start locking keys ({CacheKeys})", cacheKeys);

            var lockersCollection = new KeyLockersCollection(
                _distributedLockFactory,
                cacheKeys,
                _options.LockExpiration,
                _options.RetryEnterLockInverval,
                _logger);

            locker = lockersCollection;

            return lockersCollection.Lock(lockEnterWaitTimeout, context);
        }

        private IEnumerable<CachedValue> GetCachedValues<TId>(CacheInfo<TId>[] infos)
        {
            var cacheKeys = infos
                .Select(info => _keyFactory.CreateKey(info.Key))
                .ToArray();

            return GetWithStates(cacheKeys);
        }

        private IEnumerable<CachedValue> ObtainAndCacheRealValues<TId>(
            CacheInfo<TId>[] infos,
            DataValuesFactoryDelegate<TId, MemoryStream> dataStreamsFactory)
        {
            Debug.Assert(infos != null);

            _logger.LogInformation(
                "Start obtaining real data for missing cache keys ({CacheKeys}).",
                infos.Select(info => info.Key));

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

            _logger.LogTrace("Collect invalidation versions for tags {CacheTags}", allTagKeys);

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

                yield return (KeyState.Exist, data);

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

            FixupAndValidateExpiration(ref expiry);

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
                ITransaction transaction = CreateSetCacheTransaction(key, tags, expiry, data, conditions, out var transactionOperations);
                bool isExecuted = transaction.Execute();

                var exceptions = transactionOperations
                    .Where(operation => operation.IsFaulted)
                    .Select(operation => operation.Exception);

                if (exceptions.Any())
                {
                    throw new AggregateException(exceptions);
                }

                _logger.LogInformation(
                    "Set cache operation is finished " +
                    "(isSet: {SetTransactionStatus}, expiry: {Expiration}, key: {CacheKey}).",
                    isExecuted,
                    expiry,
                    key);

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
            IEnumerable<Condition> conditions,
            out IEnumerable<Task> transactionOperations)
        {
            ITransaction transaction = _cache.CreateTransaction();

            foreach (var condition in conditions ?? Enumerable.Empty<Condition>())
            {
                _ = transaction.AddCondition(condition);
            }

            var operations = new List<Task>();
            var dataKey = key.GetRedisKey();
            var tagExpiry = GetTagExpiry(expiry);
            var keyExpiry = expiry + _options.DeprecatedCacheTimeToLive;

            operations.Add(transaction.StringSetAsync(dataKey, data, keyExpiry));

            foreach (var tag in tags)
            {
                RedisKey tagKey = tag.GetRedisKey();
                RedisKey packTagKey = _keyFactory.CreatePack(tag).GetRedisKey();
                RedisValue tagExpiryValue = (RedisValue)(long)tagExpiry.TotalMilliseconds;

                operations.Add(transaction.SetAddAsync(tagKey, key.GetRedisValue()));

                operations.Add(transaction.ScriptEvaluateAsync(
                    ExtendKeyExpiryScript,
                    new[] { tagKey },
                    new[] { tagExpiryValue }));

                operations.Add(transaction.ScriptEvaluateAsync(
                    ExtendKeyExpiryScript,
                    new[] { packTagKey },
                    new[] { tagExpiryValue }));
            }

            transactionOperations = operations;

            return transaction;
        }

        private IEnumerable<CachedValue> GetWithStates(IReadOnlyList<CacheKey> keys)
        {
            RedisKey[] redisKeys = new RedisKey[keys.Count];
            for (int i = 0; i < keys.Count; i++)
            {
                redisKeys[i] = keys[i].GetRedisKey();
            }

            try
            {
                var cachedValues = new OperationsChain<RedisKey, CachedValue>(_logger)
                    .AddOperation(GetCachedValuesByKeys, isFinal: result => result.State == KeyState.Missing)
                    .AddOperation(MarkDeprecatedValues)
                    .Execute(redisKeys);

                _logger.LogTrace("Keys ({CacheKeys}) have values: {CacheValues}", redisKeys, cachedValues);

                return cachedValues;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get multiple caches ({CacheKeys}).", redisKeys);
                return Enumerable.Repeat(CachedValue.Empty, keys.Count);
            }
        }

        private IEnumerable<OperationResult<CachedValue>> MarkDeprecatedValues(RedisKey[] keys, OperationContext<CachedValue> context)
        {
            var ttlResultTasks = keys
                .Select(key => _cache.KeyTimeToLiveAsync(key))
                .ToList();

            for (int i = 0; i < ttlResultTasks.Count; i++)
            {
                TimeSpan? timeToLive = _cache.Wait(ttlResultTasks[i]);

                var previousValue = context.GetPreviousResult(i);

                var adjustedValue = timeToLive > _options.DeprecatedCacheTimeToLive
                    ? previousValue
                    : new CachedValue(KeyState.Deprecated, previousValue.Value);

                yield return new OperationResult<CachedValue>(adjustedValue, true);
            }
        }

        private IEnumerable<CachedValue> GetCachedValuesByKeys(IEnumerable<RedisKey> keysEnumerable)
        {
            if (keysEnumerable is not RedisKey[] keys)
            {
                keys = keysEnumerable.ToArray();
            }

            return _cache
                .StringGet(keys)
                .Select(value => value.HasValue ? new CachedValue(KeyState.Exist, (byte[])value) : CachedValue.Empty);
        }

        private void CompactTags(IReadOnlyList<CacheKey> tags, TimeSpan expiry)
        {
            RedisKey[] packKeys = tags.Select(tag => _keyFactory.CreatePack(tag.Key).GetRedisKey()).ToArray();
            RedisValue[] compactAttempts = _cache.StringGet(packKeys);

            /// TODO: Support compacting of batch of tags per one request (probably via lua script).
            /// Alternative: Use Parallel.For.
            for (int i = 0; i < tags.Count; i++)
            {
                CacheKey tag = tags[i];
                RedisKey packKey = packKeys[i];
                RedisValue compactAttempt = compactAttempts[i];

                bool isCompacted = CreateTagCompactingTransaction(tag, expiry, packKey, compactAttempt, out var transactionOperations)
                    .Execute();

                CheckCompactResult(tag, isCompacted, transactionOperations);
            }
        }

        private void CheckCompactResult(CacheKey tag, bool isCompacted, IEnumerable<Task> transactionOperations)
        {
            var exceptions = transactionOperations
                .Where(operation => operation.IsFaulted)
                .Select(operation => operation.Exception);

            if (exceptions.Any())
            {
                throw new AggregateException($"Error while compacting tag {tag}", exceptions).Flatten();
            }

            _logger.LogTrace("Tag compacting is finished (isCompacted: {CompactTransactionStatus}, tag: {CacheTag}).", isCompacted, tag);
        }

        private ITransaction CreateTagCompactingTransaction(
            CacheKey tag,
            TimeSpan expiry,
            RedisKey packKey,
            RedisValue compactAttempt,
            out IEnumerable<Task> asyncOperations)
        {
            ITransaction transaction = _cache.CreateTransaction();

            bool isPreviouslyCompacted = compactAttempt.HasValue;

            _ = transaction.AddCondition(
                Condition.SetLengthGreaterThan(tag.GetRedisKey(), _options.CompactTagSizeThreshold));

            var operations = new List<Task>();

            if (isPreviouslyCompacted && (int)compactAttempt < _options.CompactTagFrequency)
            {
                operations.Add(transaction.StringIncrementAsync(packKey));
                operations.Add(transaction.KeyExpireAsync(packKey, expiry));
            }
            else
            {
                operations.Add(transaction.StringSetAsync(packKey, 0, expiry, when: When.NotExists));
                operations.Add(transaction.ScriptEvaluateAsync(
                    CompactTagKeysScript,
                    new[] { tag.GetRedisKey() },
                    new[] { (RedisValue)(long)_options.DeprecatedCacheTimeToLive.TotalMilliseconds }));
            }

            asyncOperations = operations;

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
