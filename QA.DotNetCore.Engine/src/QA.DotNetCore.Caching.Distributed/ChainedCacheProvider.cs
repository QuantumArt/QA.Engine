using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Distributed
{
    public abstract class ChainedCacheProvider : ICacheProvider
    {
        private readonly ICacheProvider _frontCacheProvider;
        private readonly ICacheProvider _backCacheProvider;

        public ChainedCacheProvider(ICacheProvider frontCacheProvider, ICacheProvider baseCacheProvider)
        {
            _frontCacheProvider = frontCacheProvider ?? throw new ArgumentNullException(nameof(frontCacheProvider));
            _backCacheProvider = baseCacheProvider ?? throw new ArgumentNullException(nameof(baseCacheProvider));
        }

        public void Add(object data, string key, string[] tags, TimeSpan expiration)
        {
            _frontCacheProvider.Add(tags, key, tags, expiration);
            _backCacheProvider.Add(tags, key, tags, expiration);
        }

        public object Get(string key) =>
            _frontCacheProvider.Get(key) ?? _backCacheProvider.Get(key);

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

        public bool IsSet(string key) =>
            _frontCacheProvider.IsSet(key)
            || _backCacheProvider.IsSet(key);

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
    }
}
