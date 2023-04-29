using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QA.DotNetCore.Caching.Helpers.Pipes;

namespace QA.DotNetCore.Caching.Distributed
{
    public abstract class ChainedCacheProvider : ICacheProvider
    {
        private readonly ICacheProvider _frontCacheProvider;
        private readonly ICacheProvider _backCacheProvider;
        private readonly ILogger<ChainedCacheProvider> _logger;

        public ChainedCacheProvider(
            ICacheProvider frontCacheProvider,
            ICacheProvider backCacheProvider,
            ILogger<ChainedCacheProvider> logger)
        {
            _frontCacheProvider = frontCacheProvider ?? throw new ArgumentNullException(nameof(frontCacheProvider));
            _backCacheProvider = backCacheProvider ?? throw new ArgumentNullException(nameof(backCacheProvider));
            _logger = logger;
        }

        public void Add(object data, string key, string[] tags, TimeSpan expiration)
        {
            _frontCacheProvider.Add(data, key, tags, expiration);
            _backCacheProvider.Add(data, key, tags, expiration);
        }

        public IEnumerable<TResult> Get<TResult>(IEnumerable<string> keys) =>
            new Pipeline<string, TResult>(_logger)
                .AddPipe(_frontCacheProvider.Get<TResult>, isFinal: cachedValue => cachedValue != null)
                .AddPipe(_backCacheProvider.Get<TResult>)
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
            new Pipeline<string, bool>(_logger)
                .AddPipe(_frontCacheProvider.IsSet, isFinal: isSet => isSet)
                .AddPipe(_backCacheProvider.IsSet)
                .Execute(keys.ToArray());

        public bool TryGetValue<TResult>(string key, out TResult result) =>
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
