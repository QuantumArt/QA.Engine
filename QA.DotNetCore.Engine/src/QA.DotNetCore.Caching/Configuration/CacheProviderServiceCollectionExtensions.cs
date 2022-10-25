using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching.Configuration
{
    public static class CacheProviderServiceCollectionExtensions
    {
        public static void TryAddMemoryCacheServices(this IServiceCollection services)
        {
            _ = services.AddMemoryCache();
            services.TryAddSingleton<ICacheInvalidator, VersionedCacheCoreProvider>();
            services.TryAddSingleton<ICacheProvider, VersionedCacheCoreProvider>();
            services.TryAddSingleton<IMemoryCacheProvider, VersionedCacheCoreProvider>();
            services.TryAddSingleton<INodeIdentifier>(StandaloneNodeIdentifier.Instance);
            services.TryAddSingleton<IDistributedMemoryCacheProvider, VersionedCacheCoreProvider>();
        }
    }
}
