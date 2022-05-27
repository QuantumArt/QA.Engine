using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching.Distributed
{
    public class DefaultChainedCacheProvider : ChainedCacheProvider
    {
        public DefaultChainedCacheProvider(
            IDistributedMemoryCacheProvider frontCacheProvider,
            IDistributedCacheProvider baseCacheProvider)
            : base(frontCacheProvider, baseCacheProvider)
        {
        }
    }
}
