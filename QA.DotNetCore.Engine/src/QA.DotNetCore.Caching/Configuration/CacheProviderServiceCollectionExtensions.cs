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
            services.TryAddScoped<VersionedCacheCoreProvider>();
            services.TryAddScoped<ICacheProvider>(svc => svc.GetRequiredService<VersionedCacheCoreProvider>());
            services.TryAddScoped<IMemoryCacheProvider>(svc => svc.GetRequiredService<VersionedCacheCoreProvider>());
            services.TryAddScoped<ICacheInvalidator>(svc => svc.GetRequiredService<VersionedCacheCoreProvider>());
            services.TryAddScoped<IModificationStateStorage, DefaultModificationStateStorage>();
            services.TryAddScoped<ICacheKeyFactory, CacheKeyFactoryBase>();
            services.TryAddSingleton<ILockFactory, MemoryLockFactory>();
        }
    }
}
