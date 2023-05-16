using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using QA.DotNetCore.Caching.Helpers.Pipes;

namespace QA.DotNetCore.Caching
{
    public class DistributedMemoryCacheProvider : VersionedCacheCoreProvider
    {
        private static readonly TimeSpan _getUniqueIdTimout = TimeSpan.FromMinutes(1);

        private IExternalCache _externalCache;
        private readonly string _globalKeyPrefix;
        private readonly ILogger<DistributedMemoryCacheProvider> _logger;

        public DistributedMemoryCacheProvider(
            IMemoryCache cache,
            IExternalCache externalCache,
            INodeIdentifier nodeIdentifier,
            ICacheKeyFactory keyFactory,
            ILogger<DistributedMemoryCacheProvider> logger)
            : base(cache, keyFactory, logger)
        {
            using var timeoutTokenSource = new CancellationTokenSource(_getUniqueIdTimout);

            _externalCache = externalCache;
            _globalKeyPrefix = nodeIdentifier.GetUniqueId(timeoutTokenSource.Token) + ":";
            _logger = logger;
        }

        public override IEnumerable<bool> IsSet(IEnumerable<string> keys)
        {
            keys = keys.Select(GetKey).ToArray();
            
            return _externalCache.Exist(keys);
        }

        public override bool TryGetValue<TResult>(string key, out TResult result)
        {
            var keys = new[] {key};
            var exists = IsSet(keys).Single();
            result = exists ? Get<TResult>(keys).Single() : default(TResult);
            return exists;
        }

        public override IEnumerable<TResult> Get<TResult>(IEnumerable<string> keys)
        {
            keys = keys.Select(GetKey).ToArray();
            
            var localResults = base.IsSet(keys).ToArray();
            var externalResults = _externalCache.Exist(keys);
            var results = localResults.Zip(externalResults).Zip(keys)
                .Select(n => new { HasLocal = n.First.First, HasExternal = n.First.Second, Key = n.Second}).ToArray();
            var extKeys = results.Where(n => n.HasExternal && !n.HasLocal).Select(n => n.Key).ToArray();
            var dict = extKeys.Distinct().Zip(_externalCache.Get<TResult>(extKeys)).ToDictionary(n => n.First, m=> m.Second);
            foreach (var result in results)
            {
                if (!result.HasExternal)
                {
                    yield return default;
                }
                else if (result.HasLocal)
                { 
                    yield return _cache.Get<TResult>(result.Key);
                }
                else
                {
                    yield return dict[result.Key];
                }
            }
        }

        public override void Add(object data, string key, string[] tags, TimeSpan expiration)
        {
            key = GetKey(key);
            var deprecatedKey = GetDeprecatedKey(key);
            var deprecatedExpiration = TimeSpan.FromTicks(expiration.Ticks * _defaultDeprecatedCoef);
            
            if (!_externalCache.TryAdd(data, key, deprecatedKey, tags, expiration, deprecatedExpiration))
            {
                _externalCache.TryAdd(String.Empty, key, deprecatedKey, tags, expiration, deprecatedExpiration);
            }
            base.Add(data, key, tags, expiration);
        }

        public override void Invalidate(string key)
        {
            base.Invalidate(key); 
            
            _externalCache.Invalidate(GetKey(key));
        }

        public override void InvalidateByTags(params string[] tags)
        {
            base.InvalidateByTags(tags);
            
            foreach (var tag in tags)
            {
                _externalCache.InvalidateTag(GetTag(tag));
            }
        }
    }
}
