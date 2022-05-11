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
        public async Task<bool> IsExistsAsync(string key, CancellationToken token = default)
        {
            RedisKey dataKey = _keyFactory.CreateKey(key).GetRedisKey();

            await ConnectAsync(token);

            return await _cache.KeyExistsAsync(dataKey);
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default)
        {
            CacheKey dataKey = _keyFactory.CreateKey(key);

            await ConnectAsync(token);

            return await GetAsync(dataKey);
        }

        public async Task<byte[]> GetOrAddAsync(string key, string[] tags, TimeSpan expiry, Func<Task<MemoryStream>> dataStreamFactory, CancellationToken token = default)
        {
            if (tags == null)
                throw new ArgumentNullException(nameof(key));

            if (dataStreamFactory is null)
                throw new ArgumentNullException(nameof(dataStreamFactory));

            CacheKey dataKey = _keyFactory.CreateKey(key);
            IEnumerable<CacheKey> tagKeys = _keyFactory.CreateTags(tags).ToList();

            await ConnectAsync(token);

            byte[] cachedData = await GetAsync(dataKey);
            if (cachedData != null)
                return cachedData;

            IEnumerable<Condition> invalidationsState = tagKeys.Select(WatchTagInvalidation).ToList();

            RedisValue data;
            using (var dataStream = await dataStreamFactory())
                data = RedisValue.CreateFrom(dataStream);

            _ = await TrySetAsync(dataKey, tagKeys, expiry, data, invalidationsState);

            return data;
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
                throw new ArgumentNullException(nameof(key));

            CacheKey dataKey = _keyFactory.CreateKey(key);
            IEnumerable<CacheKey> tagKeys = _keyFactory.CreateTags(tags).ToList();

            await ConnectAsync(token);

            _ = await TrySetAsync(dataKey, tagKeys, expiry, RedisValue.CreateFrom(dataStream));
        }

        private async Task<bool> TrySetAsync(
            CacheKey key,
            IEnumerable<CacheKey> tags,
            TimeSpan expiry,
            RedisValue data,
            IEnumerable<Condition> conditions = null)
        {
            ITransaction transaction = CreateSetCacheTransaction(key, tags, expiry, data, conditions);
            bool result = await transaction.ExecuteAsync();

            var tagExpiry = GetTagExpiry(expiry);
            foreach (var tag in tags)
                await CompactTagAsync(tag, tagExpiry);

            return result;
        }

        private async Task ConnectAsync(CancellationToken token = default)
        {
            CheckDisposed();
            token.ThrowIfCancellationRequested();

            if (_cache != null)
                return;

            await _connectionLock.WaitAsync(token);
            try
            {
                if (_cache != null)
                    return;

                _connection = ConnectionMultiplexer.Connect(_options.Configuration);
                ValidateServerFeatures();
                _cache = _connection.GetDatabase();
            }
            finally
            {
                _ = _connectionLock.Release();
            }
        }

        private async Task<byte[]> GetAsync(CacheKey key)
        {
            RedisKey dataKey = key.GetRedisKey();
            RedisValue cachedData = await _cache.StringGetAsync(dataKey);

            if (!cachedData.HasValue)
                return default;

            return cachedData;
        }

        private async Task CompactTagAsync(CacheKey tag, TimeSpan expiry)
        {
            RedisKey packKey = _keyFactory.CreatePack(tag.Key).GetRedisKey();
            RedisValue compactAttempts = await _cache.StringGetAsync(packKey);

            ITransaction transaction = CreateTagCompactingTransaction(tag, expiry, compactAttempts);
            _ = await transaction.ExecuteAsync();
        }
    }
}
