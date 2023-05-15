using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching.Configuration;
using QA.DotNetCore.Caching.Interfaces;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;
using System;
using System.Linq;

namespace QA.DotNetCore.Caching.Distributed
{
    public static class RedisCacheServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services, Action<RedisCacheSettings> configureSettings) =>
            services.AddRedisCacheCore().Configure(configureSettings);

        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfigurationSection redisSettingsSection) =>
            services.Configure<RedisCacheSettings>(redisSettingsSection).AddRedisCacheCore();

        private static IServiceCollection AddRedisCacheCore(this IServiceCollection services)
        {
            services.TryAddSingleton<IExternalCache, RedisCache>();

            _ = services.AddSingleton<DistributedMemoryCacheProvider>();
            _ = services.AddSingleton<ICacheProvider>(svc => svc.GetRequiredService<DistributedMemoryCacheProvider>());
            _ = services.AddSingleton<ICacheInvalidator>(svc => svc.GetRequiredService<DistributedMemoryCacheProvider>());
            _ = services.AddSingleton(svc =>
                svc.GetRequiredService<IOptionsMonitor<RedisCacheSettings>>().CurrentValue);
            _ = services.AddSingleton<ExternalCacheSettings>(svc => svc.GetRequiredService<RedisCacheSettings>());
            
            _ = services.AddSingleton<ICacheKeyFactory, ExternalCacheKeyFactory>();
            _ = services.AddSingleton<IModificationStateStorage, DistributedModificationStateStorage>();

            _ = services.AddSingleton<IDistributedLockFactory>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var redisConfig = provider.GetRequiredService<RedisCacheSettings>();

                var connectionConfiguration = ConfigurationOptions.Parse(redisConfig.Configuration);
                var endPoints = connectionConfiguration.EndPoints
                    .Select(endPoint => (RedLockEndPoint)endPoint)
                    .ToList();

                return RedLockFactory.Create(
                    endPoints,
                    new(retryCount: 1),
                    loggerFactory);
            });

            services.TryAddMemoryCacheServices();

            return services;
        }
    }
}
