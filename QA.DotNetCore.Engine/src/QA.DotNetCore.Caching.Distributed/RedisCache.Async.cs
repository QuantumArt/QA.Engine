using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using QA.DotNetCore.Caching.Distributed.Internals;

namespace QA.DotNetCore.Caching.Distributed
{
    public partial class RedisCache
    {
        public async Task<IEnumerable<bool>> ExistAsync(string[] keys, CancellationToken token = default)
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
                .Select(key => new RedisKey(key))
                .ToArray();

            Stopwatch watch = Stopwatch.StartNew();

            await ConnectAsync(token);
            var existFlagsTasks = dataKeys.Select(key => _cache.KeyExistsAsync(key));
            var existFlags = await Task.WhenAll(existFlagsTasks);

            _logger.LogTrace(
                "Keys {Keys} exist {ExistFlags} (Elapsed: {Elapsed})",
                keys,
                existFlags,
                watch.ElapsedMilliseconds);

            return existFlags;
        }

        public async Task<IEnumerable<byte[]>> GetAsync(string[] keys, CancellationToken token = default)
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
                .Select(k => new RedisKey(k))
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

        private async Task<IEnumerable<CachedValue>> GetWithStatesAsync(RedisKey[] redisKeys)
        {
            try
            {
                var cachedValues = (await _cache
                        .StringGetAsync(redisKeys))
                    .Select(value =>
                        value.HasValue ? new CachedValue(KeyState.Exist, (byte[]) value) : CachedValue.Empty)
                    .ToArray();

                _logger.LogTrace("Keys ({CacheKeys}) have values: {CacheValues}", redisKeys, cachedValues);
                return cachedValues;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to get multiple caches ({CacheKeys}).", redisKeys);
                return Enumerable.Repeat(CachedValue.Empty, redisKeys.Length);
            }
        }

        public async Task InvalidateAsync(string key, CancellationToken token = default)
        {
            await ConnectAsync(token);
            var dataKey = new RedisKey(key);
            await _cache.KeyDeleteAsync(dataKey);
        }

        public async Task InvalidateTagAsync(string tag, CancellationToken token = default)
        {
            await ConnectAsync(token);
            var tagKey = new RedisKey(tag);
            await _cache.ScriptEvaluateAsync(InvalidateTagScript, new[] {tagKey});
        }

        public async Task SetAsync(string key,
            string[] tags,
            TimeSpan expiry,
            MemoryStream dataStream,
            string deprecatedKey,
            TimeSpan deprecatedExpiry,
            CancellationToken token)
        {
            RedisKey dataKey = new RedisKey(key);
            RedisKey deprecatedDataKey = new RedisKey(key);

            RedisKey[] tagKeys = tags.Select(n => new RedisKey(n)).ToArray();

            await ConnectAsync(token);

            _ = await TrySetAsync(dataKey, deprecatedDataKey, tagKeys, expiry, deprecatedExpiry,
                RedisValue.CreateFrom(dataStream));
        }

        private async Task<bool> TrySetAsync(
            RedisKey key,
            RedisKey deprecatedKey,
            IReadOnlyList<RedisKey> tags,
            TimeSpan expiry,
            TimeSpan deprecatedExpiry,
            RedisValue data,
            IEnumerable<Condition> conditions = null)
        {
            Stopwatch watch = Stopwatch.StartNew();

            try
            {
                ITransaction transaction = CreateSetCacheTransaction(key, deprecatedKey, tags, expiry, deprecatedExpiry,
                    data, conditions, out var transactionOperations);
                bool isExecuted = await transaction.ExecuteAsync();

                Exception[] exceptions = transactionOperations
                    .Where(operation => operation.IsFaulted)
                    .Select(operation => operation.Exception)
                    .ToArray();

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
    }
}
