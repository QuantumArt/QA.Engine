using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QA.DotNetCore.Caching.Helpers.Pipes;

namespace QA.DotNetCore.Caching.Distributed
{
    public partial class RedisCache
    {
        public async Task<IEnumerable<bool>> ExistAsync(IEnumerable<string> keys, CancellationToken token = default)
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

            Stopwatch watch = Stopwatch.StartNew();

            await ConnectAsync(token);

            var keyTtlTasks = dataKeys
                .Select(key => _cache.KeyTimeToLiveAsync(key))
                .ToList();

            TimeSpan?[] keyTtls = await Task.WhenAll(keyTtlTasks);

            _logger.LogTrace(
                "Keys {Keys} have ttls: {KeyTtls} (Elapsed: {Elapsed})",
                keys,
                keyTtls,
                watch.ElapsedMilliseconds);

            bool[] existFlags = keyTtls
                .Select(ttl => ttl > _options.DeprecatedCacheTimeToLive)
                .ToArray();

            _logger.LogInformation("Keys {Keys} exist {ExistFlags}", keys, existFlags);

            return existFlags;
        }

        public async Task<IEnumerable<byte[]>> GetAsync(IEnumerable<string> keys, CancellationToken token = default)
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

            Stopwatch watch = Stopwatch.StartNew();

            await ConnectAsync(token);

            var cachedValuesWithStates = await GetWithStatesAsync(dataKeys);

            var cachedData = cachedValuesWithStates
                .Select(result => result.State == KeyState.Exist ? result.Value : null)
                .ToList();

            _logger.LogTrace("Obtained cached data (Elapsed: {Elapsed})", watch.ElapsedMilliseconds);

            return cachedData;
        }

        public async Task<IReadOnlyList<byte[]>> GetOrAddAsync<TId>(
            CacheInfo<TId>[] cacheInfos,
            AsyncDataValuesFactoryDelegate<TId, MemoryStream> dataStreamsFactory,
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
                return _emptyResult;
            }

            await ConnectAsync(token);

            IAsyncDisposable locker = null;
            void SetLocker(IAsyncDisposable initializedLocker) => locker = initializedLocker;

            try
            {
                var doubleCheckLockingPipeline = new AsyncPipeline<CacheInfo<TId>, CachedValue>(_logger)
                    .AddPipe(GetCachedValuesAsync, isFinal: result => result.State == KeyState.Exist)
                    .AddPipe((infos, context) =>
                        LockOrGetDeprecatedCacheAsync(infos, context, lockEnterWaitTimeout, SetLocker))
                    .AddPipe(GetCachedValuesAsync, isFinal: result => result.State == KeyState.Exist)
                    .AddPipe(infos => ObtainAndCacheRealValuesAsync(infos, dataStreamsFactory));
                    
                return (await doubleCheckLockingPipeline.ExecuteAsync(cacheInfos))
                    .Select(result => result.Value)
                    .ToList();
            }
            finally
            {
                if (locker is not null)
                {
                    await locker.DisposeAsync();
                }
            }
        }

        private async IAsyncEnumerable<PipeOutput<CachedValue>> LockOrGetDeprecatedCacheAsync<TId>(
            CacheInfo<TId>[] infos,
            PipeContext<CachedValue> context,
            TimeSpan lockEnterWaitTimeout,
            Action<IAsyncDisposable> handleLocker)
        {
            var cacheKeys = infos.Select(info => info.Key).ToList();

            _logger.LogInformation("Start locking keys ({CacheKeys})", cacheKeys);

            var lockersCollection = new KeyLockersCollection(
                _distributedLockFactory,
                cacheKeys,
                _options.LockExpiration,
                _options.RetryEnterLockInverval,
                _logger);

            handleLocker(lockersCollection);

            foreach (var result in await lockersCollection.LockAsync(lockEnterWaitTimeout, context))
            {
                yield return result;
            }
        }

        private async IAsyncEnumerable<CachedValue> GetCachedValuesAsync<TId>(CacheInfo<TId>[] infos)
        {
            var cacheKeys = infos
                .Select(info => _keyFactory.CreateKey(info.Key))
                .ToArray();

            foreach (var cachedValue in await GetWithStatesAsync(cacheKeys))
            {
                yield return cachedValue;
            }
        }

        private async IAsyncEnumerable<CachedValue> ObtainAndCacheRealValuesAsync<TId>(
            CacheInfo<TId>[] infos,
            AsyncDataValuesFactoryDelegate<TId, MemoryStream> dataStreamsFactory)
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

            Condition[] conditions = await WatchTagInvalidationsAsync(allTagKeys);

            var conditionGroups = new IEnumerable<Condition>[infos.Length];
            for (int lastOffset = 0, i = 0; i < infos.Length; i++)
            {
                conditionGroups[i] = conditions
                    .Skip(lastOffset)
                    .Take(infos[i].Tags.Length);

                lastOffset += infos[i].Tags.Length;
            }

            // Get real data for missing cache.
            var dataStreams = await dataStreamsFactory(infos);

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

                _ = await TrySetAsync(cacheKey, tagKeys, expiry, data, invalidationsState);
            }

            CheckResultLength(recievedIndex, infos.Length, nameof(dataStreamsFactory));
        }

        public async Task InvalidateAsync(string key, CancellationToken token = default)
        {
            RedisKey dataKey = _keyFactory.CreateKey(key).GetRedisKey();

            await ConnectAsync(token);

            _ = await _cache.KeyExpireAsync(dataKey, _options.DeprecatedCacheTimeToLive);
        }

        public async Task InvalidateTagAsync(string tag, CancellationToken token = default)
        {
            CacheKey tagKey = _keyFactory.CreateTag(tag);
            RedisValue lockKey = _keyFactory.CreateLock(tagKey).GetRedisValue();
            RedisValue packPrefix = new(_keyFactory.GetPackPrefix());
            RedisValue deprecatedTtl = (RedisValue)(long)_options.DeprecatedCacheTimeToLive.TotalMilliseconds;

            await ConnectAsync(token);

            _ = await _cache.ScriptEvaluateAsync(
                InvalidateTagScript,
                new[] { tagKey.GetRedisKey() },
                new[] { lockKey, packPrefix, deprecatedTtl });
        }

        public async Task SetAsync(
            string key,
            IEnumerable<string> tags,
            TimeSpan expiry,
            MemoryStream dataStream,
            CancellationToken token = default)
        {
            FixupAndValidateExpiration(ref expiry);

            CacheKey dataKey = _keyFactory.CreateKey(key);
            CacheKey[] tagKeys = _keyFactory.CreateTags(tags).ToArray();

            await ConnectAsync(token);

            _ = await TrySetAsync(dataKey, tagKeys, expiry, RedisValue.CreateFrom(dataStream));
        }

        private async Task<Condition[]> WatchTagInvalidationsAsync(IReadOnlyList<CacheKey> tagKeys)
        {
            RedisKey[] lockKeys = new RedisKey[tagKeys.Count];
            for (int i = 0; i < tagKeys.Count; i++)
            {
                lockKeys[i] = _keyFactory.CreateLock(tagKeys[i]).GetRedisKey();
            }

            RedisValue[] lockValues = await _cache.StringGetAsync(lockKeys);

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

        private async Task<bool> TrySetAsync(
            CacheKey key,
            IReadOnlyList<CacheKey> tags,
            TimeSpan expiry,
            RedisValue data,
            IEnumerable<Condition> conditions = null)
        {
            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                ITransaction transaction = CreateSetCacheTransaction(key, tags, expiry, data, conditions, out var transactionOperations);
                bool isExecuted = await transaction.ExecuteAsync();

                var exceptions = transactionOperations
                    .Where(operation => operation.IsFaulted)
                    .Select(operation => operation.Exception);

                if (exceptions.Any())
                {
                    throw new AggregateException(exceptions);
                }

                _logger.LogInformation(
                    "Set cache operation is finished " +
                    "(isSet: {SetTransactionStatus}, expiry: {Expiration}, key: {CacheKey}, elapsed: {Elapsed})).",
                    isExecuted,
                    expiry,
                    key,
                    watch.ElapsedMilliseconds);

                if (isExecuted)
                {
                    var tagExpiry = GetTagExpiry(expiry);
                    await CompactTagsAsync(tags, tagExpiry);
                }

                return isExecuted;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unable to set cache with the key {CacheKey} (Elapsed: {Elapsed})",
                    key,
                    watch.ElapsedMilliseconds);

                return false;
            }
        }

        private async Task ConnectAsync(CancellationToken token = default)
        {
            CheckDisposed();
            token.ThrowIfCancellationRequested();

            if (_cache != null)
            {
                return;
            }

            await _connectionLock.WaitAsync(token);
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

        private async Task<IEnumerable<CachedValue>> GetWithStatesAsync(IReadOnlyList<CacheKey> keys)
        {
            RedisKey[] redisKeys = new RedisKey[keys.Count];
            for (int i = 0; i < keys.Count; i++)
            {
                redisKeys[i] = keys[i].GetRedisKey();
            }

            try
            {
                var cachedValues = await new AsyncPipeline<RedisKey, CachedValue>(_logger)
                    .AddPipe(GetCachedValuesByKeysAsync, isFinal: result => result.State == KeyState.Missing)
                    .AddPipe(MarkDeprecatedValuesAsync)
                    .ExecuteAsync(redisKeys);

                _logger.LogTrace("Keys ({CacheKeys}) have values: {CacheValues}", redisKeys, cachedValues);

                return cachedValues;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get multiple caches ({CacheKeys}).", redisKeys);
                return Enumerable.Repeat(CachedValue.Empty, keys.Count);
            }
        }

        private async IAsyncEnumerable<PipeOutput<CachedValue>> MarkDeprecatedValuesAsync(RedisKey[] keys, PipeContext<CachedValue> context)
        {
            var ttlResultTasks = keys
                .Select(key => _cache.KeyTimeToLiveAsync(key))
                .ToList();

            for (int i = 0; i < ttlResultTasks.Count; i++)
            {
                TimeSpan? timeToLive = await ttlResultTasks[i];

                var previousValue = context.GetPreviousResult(i);

                var adjustedValue = timeToLive > _options.DeprecatedCacheTimeToLive
                    ? previousValue
                    : new CachedValue(KeyState.Deprecated, previousValue.Value);

                yield return new PipeOutput<CachedValue>(adjustedValue, true);
            }
        }

        private async IAsyncEnumerable<CachedValue> GetCachedValuesByKeysAsync(IEnumerable<RedisKey> keysEnumerable)
        {
            if (keysEnumerable is not RedisKey[] keys)
            {
                keys = keysEnumerable.ToArray();
            }

            var cachedValues = await _cache.StringGetAsync(keys);

            foreach (var value in cachedValues)
            {
                yield return value.HasValue
                    ? new CachedValue(KeyState.Exist, (byte[])value)
                    : CachedValue.Empty;
            }
        }

        private async Task CompactTagsAsync(IReadOnlyList<CacheKey> tags, TimeSpan expiry)
        {
            Stopwatch watch = Stopwatch.StartNew();

            RedisKey[] packKeys = tags.Select(tag => _keyFactory.CreatePack(tag.Key).GetRedisKey()).ToArray();
            RedisValue[] compactAttempts = await _cache.StringGetAsync(packKeys);

            var transactionExecutions = new Task[tags.Count];
            for (int i = 0; i < tags.Count; i++)
            {
                CacheKey tag = tags[i];
                RedisKey packKey = packKeys[i];
                RedisValue compactAttempt = compactAttempts[i];

                transactionExecutions[i] = CompactTagAsync(expiry, tag, packKey, compactAttempt);
            }

            await Task.WhenAll(transactionExecutions);

            _logger.LogTrace("Tags compacting is finished (Elapsed: {Elapsed})", watch.ElapsedMilliseconds);
        }

        private async Task CompactTagAsync(TimeSpan expiry, CacheKey tag, RedisKey packKey, RedisValue compactAttempt)
        {
            if ((int) compactAttempt >= _options.CompactTagFrequency)
            {
                var isCompacted = await CreateTagCompactingTransaction(tag, expiry, packKey, out var transactionOperations)
                    .ExecuteAsync();
                CheckCompactResult(tag, isCompacted, transactionOperations);
            }
            else
            {
                await Task.CompletedTask;
            }
        }
    }
}
