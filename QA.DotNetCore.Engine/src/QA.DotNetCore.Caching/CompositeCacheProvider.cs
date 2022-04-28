using Microsoft.Extensions.Caching.Memory;
using QA.DotNetCore.Caching.Interfaces;
using System;

namespace QA.DotNetCore.Caching
{
    public class CompositeCacheProvider : VersionedCacheCoreProvider, ICompositeCacheProvider
    {
        private readonly ICacheProvider _globalCacheProvider;
        private readonly string _globalKeyPrefix;

        public CompositeCacheProvider(
            IMemoryCache cache,
            ICacheProvider globalCacheProvider,
            INodeIdentifier nodeIdentifier)
            : base(cache)
        {
            _globalCacheProvider = globalCacheProvider;
            _globalKeyPrefix = nodeIdentifier.GetUniqueId() + ":";
        }

        public override object Get(string key)
        {
            object cachedValue = base.Get(key);
            return _globalCacheProvider.IsSet(GetGlobalKey(key)) ? cachedValue : null;
        }

        public override void Add(object data, string key, string[] tags, TimeSpan expiration)
        {
            _globalCacheProvider.Add(true, GetGlobalKey(key), tags, expiration);
            base.Add(data, key, tags, expiration);
        }

        private string GetGlobalKey(string key) => _globalKeyPrefix + key;
    }
}
