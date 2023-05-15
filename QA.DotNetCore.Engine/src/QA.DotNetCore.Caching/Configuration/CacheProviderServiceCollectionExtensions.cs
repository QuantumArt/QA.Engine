using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching.Configuration
{
    public static class CacheProviderServiceCollectionExtensions
    {
        public static void TryAddMemoryCacheServices(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.TryAddSingleton<VersionedCacheCoreProvider>();
            services.TryAddSingleton<ICacheProvider>(svc => svc.GetRequiredService<VersionedCacheCoreProvider>());
            services.TryAddSingleton<IMemoryCacheProvider>(svc => svc.GetRequiredService<VersionedCacheCoreProvider>());
            services.TryAddSingleton<ICacheInvalidator>(svc => svc.GetRequiredService<VersionedCacheCoreProvider>());
            
            services.TryAddSingleton<ICacheKeyFactory, CacheKeyFactoryBase>();
            services.TryAddSingleton<ICacheInvalidator>(svc => svc.GetService<ICacheProvider>());
            services.TryAddSingleton<INodeIdentifier>(StandaloneNodeIdentifier.Instance);
            services.TryAddSingleton<IModificationStateStorage, DefaultModificationStateStorage>();
        }
    }
}
