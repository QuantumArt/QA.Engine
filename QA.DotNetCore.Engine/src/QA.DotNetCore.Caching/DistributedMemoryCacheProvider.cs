using Microsoft.Extensions.Caching.Memory;
using QA.DotNetCore.Caching.Interfaces;
using System;

namespace QA.DotNetCore.Caching
{
    public class DistributedMemoryCacheProvider : VersionedCacheCoreProvider, IDistributedMemoryCacheProvider
    {
        private readonly IDistributedCacheProvider _distributedCacheProvider;
        private readonly string _globalKeyPrefix;

        public DistributedMemoryCacheProvider(
            IMemoryCache cache,
            IDistributedCacheProvider globalCacheProvider,
            INodeIdentifier nodeIdentifier)
            : base(cache)
        {
            _distributedCacheProvider = globalCacheProvider;
            _globalKeyPrefix = nodeIdentifier.GetUniqueId() + ":";
        }

        public override bool IsSet(string key) =>
            base.IsSet(key) && _distributedCacheProvider.IsSet(GetGlobalKey(key));

        public override object Get(string key)
        {
            object cachedValue = base.Get(key);

            if (cachedValue == null || !_distributedCacheProvider.IsSet(GetGlobalKey(key)))
            {
                return null;
            }

            return cachedValue;
        }

        public override void Add(object data, string key, string[] tags, TimeSpan expiration)
        {
            _distributedCacheProvider.Add(string.Empty, GetGlobalKey(key), tags, expiration);
            base.Add(data, key, tags, expiration);
        }

        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.", true)]
        public override void Invalidate(string key)
        {
            base.Invalidate(key);
            _distributedCacheProvider.Invalidate(key);
        }

        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.", true)]
        public override void InvalidateByTags(params string[] tags)
        {
            base.InvalidateByTags(tags);
            _distributedCacheProvider.InvalidateByTags(tags);
        }

        private string GetGlobalKey(string key) => _globalKeyPrefix + key;
    }
}
