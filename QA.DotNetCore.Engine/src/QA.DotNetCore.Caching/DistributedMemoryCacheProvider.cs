using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Helpers.Operations;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Caching
{
    public class DistributedMemoryCacheProvider : VersionedCacheCoreProvider, IDistributedMemoryCacheProvider
    {
        private readonly IDistributedCacheProvider _distributedCacheProvider;
        private readonly ILogger<DistributedMemoryCacheProvider> _logger;
        private readonly string _globalKeyPrefix;

        public DistributedMemoryCacheProvider(
            IMemoryCache cache,
            IDistributedCacheProvider globalCacheProvider,
            INodeIdentifier nodeIdentifier,
            ILogger<DistributedMemoryCacheProvider> logger)
            : base(cache, logger)
        {
            _distributedCacheProvider = globalCacheProvider;
            _globalKeyPrefix = nodeIdentifier.GetUniqueId() + ":";
            _logger = logger;
        }

        public override IEnumerable<bool> IsSet(IEnumerable<string> keys)
        {
            return new OperationsChain<string, bool>(_logger)
                .AddOperation(base.IsSet, isFinal: isSet => !isSet)
                .AddOperation(_distributedCacheProvider.IsSet)
                .Execute(keys.ToArray());
        }

        public override IEnumerable<object> Get(IEnumerable<string> keys)
        {
            IEnumerable<OperationResult<object>> VerifyCacheIsGloballySet(string[] cachedKeys, OperationContext<object> context)
            {
                var globalSetResults = _distributedCacheProvider.IsSet(cachedKeys.Select(GetGlobalKey));
                using var globallySetEnumerator = globalSetResults.GetEnumerator();

                for (int i = 0; globallySetEnumerator.MoveNext(); i++)
                {
                    bool isSetGlobally = globallySetEnumerator.Current;
                    object result = isSetGlobally ? context.GetPreviousResult(i) : null;

                    yield return new OperationResult<object>(result, isSetGlobally);
                }
            }

            return new OperationsChain<string, object>(_logger)
                .AddOperation(base.Get, isFinal: cachedValue => cachedValue is null)
                .AddOperation(VerifyCacheIsGloballySet)
                .Execute(keys.ToArray());
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
