using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Configuration;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces;

namespace QA.DotNetCore.Engine.CacheTags.Configuration
{
    public static class MvcServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление сервисов для работы кештегов в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        /// <param name="setupAction">конфигуратор настроек</param>
        public static ICacheTagConfigurationBuilder AddCacheTagServices(
            this IServiceCollection services)
        {
            var cfg = new CacheTagsRegistrationConfigurator();
            services.TryAddSingleton(provider =>
                provider.GetRequiredService<IOptions<CacheTagsRegistrationConfigurator>>().Value);

            services.TryAddMemoryCacheServices();
            services.TryAddScoped<ICacheTagWatcher, CacheTagWatcher>();
            services.TryAddScoped<IQpContentCacheTagNamingProvider, DefaultQpContentCacheTagNamingProvider>();
            services.TryAddScoped<QpContentCacheTracker>();
            services.TryAddScoped<IContentModificationRepository, ContentModificationRepository>();
            services.TryAddSingleton<ICacheTrackersAccessor, CacheTrackersAccessor>();

            services.TryAddSingleton(provider =>
                provider.GetRequiredService<IOptions<ServiceSetConfigurator<ICacheTagTracker>>>().Value);

            return new CacheTagConfigurationBuilder(services);
        }

        public static ICacheTagConfigurationBuilder WithInvalidationByEvent<TEvent, TConsumer, TFaultConsumer>(
            this ICacheTagConfigurationBuilder builder)
            where TEvent : class
            where TConsumer : class, IConsumer<TEvent>
            where TFaultConsumer : class, IConsumer<Fault<TEvent>>
        {
            _ = builder.Services.AddMassTransit(registrationConfig =>
            {
                _ = registrationConfig.AddConsumer<TConsumer>();
                _ = registrationConfig.AddConsumer<TFaultConsumer>();

                registrationConfig.UsingRabbitMq((context, factoryConfig) =>
                {
                    var rabbitMqSettings = context.GetRequiredService<IOptions<RabbitMqSettings>>().Value;

                    factoryConfig.UseRetry(retry => retry.Interval(
                        rabbitMqSettings.RetryLimit,
                        rabbitMqSettings.RetryDelay));

                    factoryConfig.Host(
                        rabbitMqSettings.Host,
                        rabbitMqSettings.Port,
                        rabbitMqSettings.VirtualPath,
                        hostConfig =>
                        {
                            hostConfig.Username(rabbitMqSettings.Username);
                            hostConfig.Password(rabbitMqSettings.Password);
                            hostConfig.Heartbeat(rabbitMqSettings.Heartbeat);
                        });

                    factoryConfig.ConfigureEndpoints(context);
                });
            });

            return builder;
        }

        public static ICacheTagConfigurationBuilder WithInvalidationByTimer(
            this ICacheTagConfigurationBuilder builder, TimeSpan? interval = null)
        {
            _ = builder.Services.Configure<CacheTagsRegistrationConfigurator>(options =>
                options.InvalidateByTimer(interval));
            _ = builder.Services.AddSingleton<IHostedService, CacheInvalidationService>();

            return builder;
        }

        public static ICacheTagConfigurationBuilder WithInvalidationByMiddleware(
            this ICacheTagConfigurationBuilder builder,
            string? excludeRequestPathRegex)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            _ = builder.Services.Configure<CacheTagsRegistrationConfigurator>(options =>
                options.InvalidateByMiddleware(excludeRequestPathRegex));

            return builder;
        }

        public static ICacheTagConfigurationBuilder WithCacheTrackers(
            this ICacheTagConfigurationBuilder builder,
            Action<ServiceSetConfigurator<ICacheTagTracker>> configureTrackers)
        {
            if (builder is null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (configureTrackers is null)
            {
                throw new ArgumentNullException(nameof(configureTrackers));
            }

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
