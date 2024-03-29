﻿using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching.Configuration;
using QA.DotNetCore.Caching.Distributed.Internals;
using QA.DotNetCore.Caching.Interfaces;
using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;
using StackExchange.Redis;

namespace QA.DotNetCore.Caching.Distributed.Configuration
{
    public static class RedisCacheServiceCollectionExtensions
    {
        public static IServiceCollection AddRedisCache(this IServiceCollection services,
            Action<RedisCacheSettings> configureSettings) =>
            services.AddRedisCacheCore().Configure(configureSettings);

        public static IServiceCollection AddRedisCache(this IServiceCollection services,
            IConfigurationSection redisSettingsSection) =>
            services.Configure<RedisCacheSettings>(redisSettingsSection).AddRedisCacheCore();

        private static IServiceCollection AddRedisCacheCore(this IServiceCollection services)
        {
            services.TryAddSingleton<IExternalCache, RedisCache>();
            _ = services.AddSingleton(svc =>
                svc.GetRequiredService<IOptionsMonitor<RedisCacheSettings>>().CurrentValue);
            _ = services.AddSingleton<ExternalCacheSettings>(svc => svc.GetRequiredService<RedisCacheSettings>());

            _ = services.AddScoped<DistributedMemoryCacheProvider>();
            _ = services.AddScoped<ICacheProvider>(svc => svc.GetRequiredService<DistributedMemoryCacheProvider>());
            _ = services.AddScoped<ICacheInvalidator>(svc => svc.GetRequiredService<DistributedMemoryCacheProvider>());
            _ = services.AddScoped<ICacheKeyFactory, ExternalCacheKeyFactory>();
            _ = services.AddSingleton<ExternalLockFactory>();
            _ = services.AddSingleton<ILockFactory>(svc =>
            {
                var settings = svc.GetRequiredService<ExternalCacheSettings>();
                return settings.UseExternalLock
                    ? svc.GetRequiredService<ExternalLockFactory>()
                    : svc.GetRequiredService<MemoryLockFactory>();
            });

            _ = services.AddSingleton<IDistributedLockFactory>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var redisConfig = provider.GetRequiredService<RedisCacheSettings>();

                var connectionConfiguration = ConfigurationOptions.Parse(redisConfig.Configuration);

                var endPoints = connectionConfiguration.EndPoints
                    .Select(endPoint => new RedLockEndPoint(endPoint)
                    {
                        Password = connectionConfiguration.Password,
                        Ssl = connectionConfiguration.Ssl,
                        ConnectionTimeout = connectionConfiguration.ConnectTimeout,
                        SyncTimeout = connectionConfiguration.SyncTimeout
                    })
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
