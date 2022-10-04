using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Helpers.Operations;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace QA.DotNetCore.Caching
{
    public class DistributedMemoryCacheProvider : VersionedCacheCoreProvider, IDistributedMemoryCacheProvider
    {
        private static readonly TimeSpan _getUniqueIdTimout = TimeSpan.FromMinutes(1);

        private readonly IDistributedCacheProvider _distributedCacheProvider;
        private readonly ICacheInvalidator _distributedCacheInvalidator;
        private readonly ILogger<DistributedMemoryCacheProvider> _logger;
        private readonly string _globalKeyPrefix;

        private ICacheInvalidator Invalidator => this;

        public DistributedMemoryCacheProvider(
            IMemoryCache cache,
            IDistributedCacheProvider distributedCacheProvider,
            ICacheInvalidator distributedCacheInvalidator,
            INodeIdentifier nodeIdentifier,
            ILogger<DistributedMemoryCacheProvider> logger)
            : base(cache, logger)
        {
            using var timeoutTokenSource = new CancellationTokenSource(_getUniqueIdTimout);

            _distributedCacheProvider = distributedCacheProvider;
            _distributedCacheInvalidator = distributedCacheInvalidator;
            _globalKeyPrefix = nodeIdentifier.GetUniqueId(timeoutTokenSource.Token) + ":";
            _logger = logger;
        }

        public override IEnumerable<bool> IsSet(IEnumerable<string> keys)
        {
            return new OperationsChain<string, bool>(_logger)
                .AddOperation(base.IsSet, isFinal: isSet => !isSet)
                .AddOperation(_distributedCacheProvider.IsSet)
                .Execute(keys.ToArray());
        }

        public override IEnumerable<TResult> Get<TResult>(IEnumerable<string> keys)
        {
            IEnumerable<OperationResult<TResult>> VerifyCacheIsGloballySet(string[] cachedKeys, OperationContext<TResult> context)
            {
                var globalSetResults = _distributedCacheProvider.IsSet(cachedKeys.Select(GetGlobalKey));
                using var globallySetEnumerator = globalSetResults.GetEnumerator();

                for (int i = 0; globallySetEnumerator.MoveNext(); i++)
                {
                    bool isSetGlobally = globallySetEnumerator.Current;
                    TResult result = isSetGlobally ? context.GetPreviousResult(i) : default;

                    yield return new OperationResult<TResult>(result, isSetGlobally);
                }
            }

            return new OperationsChain<string, TResult>(_logger)
                .AddOperation(base.Get<TResult>, isFinal: cachedValue => cachedValue is null)
                .AddOperation(VerifyCacheIsGloballySet)
                .Execute(keys.ToArray());
        }

        public override void Add(object data, string key, string[] tags, TimeSpan expiration)
        {
            tags = tags.Append(key).ToArray();
            _distributedCacheInvalidator.InvalidateByTags(key);

            _distributedCacheProvider.Add(string.Empty, GetGlobalKey(key), tags, expiration);
            base.Add(data, key, tags, expiration);
        }

        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.", true)]
        public override void Invalidate(string key)
        {
            base.Invalidate(key);
            _distributedCacheInvalidator.Invalidate(key);
        }

        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.", true)]
        public override void InvalidateByTags(params string[] tags)
        {
            base.InvalidateByTags(tags);
            _distributedCacheInvalidator.InvalidateByTags(tags);
        }

        private string GetGlobalKey(string key) => _globalKeyPrefix + key;
    }
}
