using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching.Configuration
{
    public static class CacheProviderServiceCollectionExtensions
    {
        public static void TryAddMemoryCacheServices(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.TryAddScoped(svc =>
                new VersionedCacheCoreProvider(
                    svc.GetRequiredService<IMemoryCache>(),
                    svc.GetRequiredService<ICacheKeyFactory>(),
                    svc.GetRequiredService<MemoryLockFactory>()
                )
            );
            services.TryAddScoped<ICacheProvider>(svc => svc.GetRequiredService<VersionedCacheCoreProvider>());
            services.TryAddScoped<IMemoryCacheProvider>(svc => svc.GetRequiredService<VersionedCacheCoreProvider>());
            services.TryAddScoped<ICacheInvalidator>(svc => svc.GetRequiredService<VersionedCacheCoreProvider>());
            services.TryAddScoped<ICacheKeyFactory, CacheKeyFactoryBase>();

            services.TryAddSingleton<IModificationStateStorage, DefaultModificationStateStorage>();
            services.TryAddSingleton<MemoryLockFactory>();
            services.TryAddSingleton<ILockFactory>(svc => svc.GetRequiredService<MemoryLockFactory>());
            services.TryAddSingleton(_ => new CleanCacheLockerServiceSettings());
            services.AddHostedService<CleanCacheLockerService>();
        }
    }
}
