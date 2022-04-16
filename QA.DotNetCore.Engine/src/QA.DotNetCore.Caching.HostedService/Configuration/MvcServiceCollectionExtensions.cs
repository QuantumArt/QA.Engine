using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using System;

namespace QA.DotNetCore.Engine.CacheTags.Configuration
{
    public static class MvcServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление сервисов для работы кештегов в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        /// <param name="setupAction">конфигуратор настроек</param>
        [Obsolete("Use " + nameof(AddCacheTagServices) + " override that returns builder instead.")]
        public static void AddCacheTagServices(this IServiceCollection services, Action<CacheTagsRegistrationConfigurator> setupAction)
        {
            var cfg = new CacheTagsRegistrationConfigurator();
            setupAction?.Invoke(cfg);
            services.AddSingleton(provider => provider.GetRequiredService<IOptions<CacheTagsRegistrationConfigurator>>().Value);

            services.AddSingleton<ICacheTagWatcher, CacheTagWatcher>();
            services.AddScoped<IQpContentCacheTagNamingProvider, DefaultQpContentCacheTagNamingProvider>();
            services.AddScoped<QpContentCacheTracker>();
            services.AddScoped<IContentModificationRepository, ContentModificationRepository>();
            services.AddSingleton<ICacheTrackersAccessor, CacheTrackersAccessor>();
            services.AddSingleton<ServiceSetConfigurator<ICacheTagTracker>>();

            if (cfg.UseTimer)
            {
                services.AddSingleton<IHostedService, CacheInvalidationService>();
            }
        }

        /// <summary>
        /// Добавление сервисов для работы кештегов в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        /// <param name="setupAction">конфигуратор настроек</param>
        public static ICacheTagConfigurationBuilder AddCacheTagServices(
            this IServiceCollection services)
        {
            var cfg = new CacheTagsRegistrationConfigurator();
            services.TryAddSingleton(provider => provider.GetRequiredService<IOptions<CacheTagsRegistrationConfigurator>>().Value);

            services.TryAddSingleton<ICacheTagWatcher, CacheTagWatcher>();
            services.TryAddScoped<IQpContentCacheTagNamingProvider, DefaultQpContentCacheTagNamingProvider>();
            services.TryAddScoped<QpContentCacheTracker>();
            services.TryAddScoped<IContentModificationRepository, ContentModificationRepository>();
            services.TryAddSingleton<ICacheTrackersAccessor, CacheTrackersAccessor>();

            services.TryAddSingleton(provider => provider.GetRequiredService<IOptions<ServiceSetConfigurator<ICacheTagTracker>>>().Value);

            return new CacheTagConfigurationBuilder(services);
        }

        public static ICacheTagConfigurationBuilder WithInvalidationByTimer(
            this ICacheTagConfigurationBuilder builder)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            _ = builder.Services.Configure<CacheTagsRegistrationConfigurator>(options => options.InvalidateByTimer());
            _ = builder.Services.AddSingleton<IHostedService, CacheInvalidationService>();

            return builder;
        }

        public static ICacheTagConfigurationBuilder WithInvalidationByMiddleware(
            this ICacheTagConfigurationBuilder builder,
            string excludeRequestPathRegex)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            _ = builder.Services.Configure<CacheTagsRegistrationConfigurator>(options => options.InvalidateByMiddleware(excludeRequestPathRegex));

            return builder;
        }

        public static ICacheTagConfigurationBuilder WithCacheTrackers(
            this ICacheTagConfigurationBuilder builder,
            Action<ServiceSetConfigurator<ICacheTagTracker>> configureTrackers)
        {
            if (builder is null)
                throw new ArgumentNullException(nameof(builder));

            if (configureTrackers is null)
                throw new ArgumentNullException(nameof(configureTrackers));

            _ = builder.Services.Configure(configureTrackers);

            return builder;
        }

        private class CacheTagConfigurationBuilder : ICacheTagConfigurationBuilder
        {
            public IServiceCollection Services { get; }

            public CacheTagConfigurationBuilder(IServiceCollection services)
            {
                Services = services ?? throw new ArgumentNullException(nameof(services));
            }
        }
    }
}
