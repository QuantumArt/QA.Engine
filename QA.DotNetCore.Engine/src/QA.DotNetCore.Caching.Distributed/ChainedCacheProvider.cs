using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Helpers.Operations;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Distributed
{
    public abstract class ChainedCacheProvider : ICacheProvider
    {
        private readonly ICacheProvider _frontCacheProvider;
        private readonly ICacheProvider _backCacheProvider;
        private readonly ILogger<ChainedCacheProvider> _logger;

        public ChainedCacheProvider(
            ICacheProvider frontCacheProvider,
            ICacheProvider baseCacheProvider,
            ILogger<ChainedCacheProvider> logger)
        {
            _frontCacheProvider = frontCacheProvider ?? throw new ArgumentNullException(nameof(frontCacheProvider));
            _backCacheProvider = baseCacheProvider ?? throw new ArgumentNullException(nameof(baseCacheProvider));
            _logger = logger;
        }

        public void Add(object data, string key, string[] tags, TimeSpan expiration)
        {
            _frontCacheProvider.Add(tags, key, tags, expiration);
            _backCacheProvider.Add(tags, key, tags, expiration);
        }

        public IEnumerable<object> Get(IEnumerable<string> keys) =>
            new OperationsChain<string, object>(_logger)
                .AddOperation(_frontCacheProvider.Get, isFinal: cachedValue => cachedValue != null)
                .AddOperation(_backCacheProvider.Get)
                .Execute(keys.ToArray());

        public T GetOrAdd<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<T> getValue, TimeSpan waitForCalculateTimeout = default)
        {
            return _frontCacheProvider.GetOrAdd(
                cacheKey,
                tags,
                expiration,
                () => _backCacheProvider.GetOrAdd(
                    cacheKey,
                    tags,
                    expiration,
                    getValue,
                    waitForCalculateTimeout),
                waitForCalculateTimeout);
        }

        public Task<T> GetOrAddAsync<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<Task<T>> getData, TimeSpan waitForCalculateTimeout = default)
        {
            return _frontCacheProvider.GetOrAdd(
                cacheKey,
                tags,
                expiration,
                () => _backCacheProvider.GetOrAdd(
                    cacheKey,
                    tags,
                    expiration,
                    getData,
                    waitForCalculateTimeout),
                waitForCalculateTimeout);
        }

        public IEnumerable<bool> IsSet(IEnumerable<string> keys) =>
            new OperationsChain<string, bool>(_logger)
                .AddOperation(_frontCacheProvider.IsSet, isFinal: isSet => isSet)
                .AddOperation(_backCacheProvider.IsSet)
                .Execute(keys.ToArray());

        public bool TryGetValue(string key, out object result) =>
            _frontCacheProvider.TryGetValue(key, out result)
            || _backCacheProvider.TryGetValue(key, out result);

        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.", true)]
        public void Invalidate(string key)
        {
            _frontCacheProvider.Invalidate(key);
            _backCacheProvider.Invalidate(key);
        }

        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.", true)]
        public void InvalidateByTags(params string[] tags)
        {
            _frontCacheProvider.InvalidateByTags(tags);
            _backCacheProvider.InvalidateByTags(tags);
        }

        public TResult[] GetOrAdd<TId, TResult>(
            CacheInfo<TId>[] cacheInfos,
            DataValuesFactoryDelegate<TId, TResult> dataValuesFactory,
            TimeSpan waitForCalculateTimeout = default)
        {
            return _frontCacheProvider.GetOrAdd(
                cacheInfos,
                (infos) => _backCacheProvider.GetOrAdd(
                    infos.ToArray(),
                    dataValuesFactory,
                    waitForCalculateTimeout),
                waitForCalculateTimeout);
        }
    }
}
