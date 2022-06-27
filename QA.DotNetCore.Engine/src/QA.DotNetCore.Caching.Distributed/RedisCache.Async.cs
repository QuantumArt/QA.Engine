using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Helpers.Operations;
using QA.DotNetCore.Caching.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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

            Connect(token);

            if (dataKeys.Length == 1)
            {
                return new[] { _cache.KeyExists(dataKeys[0]) };
            }

            var results = await _cache.ScriptEvaluateAsync(GetMultipleKeysExistance, dataKeys);

            return (bool[])results;
        }

        public async Task<IEnumerable<byte[]>> GetAsync(IEnumerable<string> keys, CancellationToken token = default)
        {
            if (keys is null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            if (!keys.Any())
            {
                return s_emptyResult;
            }

            var dataKeys = keys
                .Select(_keyFactory.CreateKey)
                .ToArray();

            await ConnectAsync(token);

            return await InnerGetAsync(dataKeys);
        }

        public async Task<IReadOnlyList<byte[]>> GetOrAddAsync<TId>(
            CacheInfo<TId>[] cacheInfos,
            AsyncDataValuesFactoryDelegate<TId, MemoryStream> dataStreamsFactory,
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

            var results = await new AsyncOperationsChain<CacheInfo<TId>, byte[]>()
                .AddOperation(GetCachedValuesAsync, isFinal: result => result != null)
                .AddOperation((infos) => ObtainAndCacheRealValuesAsync(infos, dataStreamsFactory))
                .ExecuteAsync(cacheInfos);

            return results.ToList();
        }

        private async IAsyncEnumerable<byte[]> GetCachedValuesAsync<TId>(IEnumerable<CacheInfo<TId>> infos)
        {
            CacheKey[] cacheKeys = infos
                .Select(info => _keyFactory.CreateKey(info.Key))
                .ToArray();

            foreach (var cachedValue in await InnerGetAsync(cacheKeys))
            {
                yield return cachedValue;
            }
        }

        private async IAsyncEnumerable<byte[]> ObtainAndCacheRealValuesAsync<TId>(
            CacheInfo<TId>[] infos,
            AsyncDataValuesFactoryDelegate<TId, MemoryStream> dataStreamsFactory)
        {
            // TODO: Extract common between sync and async implementation.
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

                yield return data;

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

            _ = await _cache.KeyDeleteAsync(dataKey);
        }

        public async Task InvalidateTagAsync(string tag, CancellationToken token = default)
        {
            CacheKey tagKey = _keyFactory.CreateTag(tag);
            RedisValue lockKey = _keyFactory.CreateLock(tagKey).GetRedisValue();

            await ConnectAsync(token);

            _ = await _cache.ScriptEvaluateAsync(
                InvalidateTagScript,
                new[] { tagKey.GetRedisKey() },
                new[] { lockKey });
        }

        public async Task SetAsync(string key, IEnumerable<string> tags, TimeSpan expiry, MemoryStream dataStream, CancellationToken token = default)
        {
            if (tags == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

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
            try
            {
                ITransaction transaction = CreateSetCacheTransaction(key, tags, expiry, data, conditions);
                bool isExecuted = await transaction.ExecuteAsync();

                if (isExecuted)
                {
                    var tagExpiry = GetTagExpiry(expiry);
                    await CompactTagsAsync(tags, tagExpiry);
                }

                return isExecuted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to set cache with the key {CacheKey}", key);
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

        private async Task<IEnumerable<byte[]>> InnerGetAsync(IReadOnlyList<CacheKey> keys)
        {
            RedisKey[] redisKeys = new RedisKey[keys.Count];
            try
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    redisKeys[i] = keys[i].GetRedisKey();
                }

                var redisResults = await _cache.StringGetAsync(redisKeys);

                return redisResults.Select(result => result.HasValue ? (byte[])result : null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get multiple caches ('{0}').", redisKeys);
                return Enumerable.Repeat(Array.Empty<byte>(), keys.Count);
            }
        }

        private async Task CompactTagsAsync(IReadOnlyList<CacheKey> tags, TimeSpan expiry)
        {
            RedisKey[] packKeys = tags.Select(tag => _keyFactory.CreatePack(tag.Key).GetRedisKey()).ToArray();
            RedisValue[] compactAttempts = await _cache.StringGetAsync(packKeys);

            ITransaction transaction = CreateTagCompactingTransaction(tags, expiry, packKeys, compactAttempts);
            _ = await transaction.ExecuteAsync();
        }
    }
}
