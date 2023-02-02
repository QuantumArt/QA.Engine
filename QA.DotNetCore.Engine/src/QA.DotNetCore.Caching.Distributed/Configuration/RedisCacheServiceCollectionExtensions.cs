﻿using Microsoft.Extensions.Configuration;
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
            services.AddRedisCacheCore().Configure<RedisCacheSettings>(redisSettingsSection);

        private static IServiceCollection AddRedisCacheCore(this IServiceCollection services)
        {
            services.TryAddSingleton<IDistributedTaggedCache, RedisCache>();

            _ = services.AddSingleton<IDistributedCacheProvider, RedisCacheProvider>();
            _ = services.AddSingleton<IDistributedMemoryCacheProvider, DistributedMemoryCacheProvider>();
            _ = services.AddSingleton<ICacheProvider, DefaultChainedCacheProvider>();

            _ = services.AddSingleton<ICacheInvalidator, RedisCacheProvider>();
            _ = services.AddSingleton<IModificationStateStorage, DistributedModificationStateStorage>();
            _ = services.AddSingleton<INodeIdentifier, RedisNodeIdentifier>();

            _ = services.AddSingleton<IDistributedLockFactory>(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                var options = provider.GetRequiredService<IOptionsMonitor<RedisCacheSettings>>();

                var connectionConfiguration = ConfigurationOptions.Parse(options.CurrentValue.Configuration);
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