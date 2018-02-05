using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Interfaces;
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
        public static void AddCacheTagServices(this IServiceCollection services, Action<CacheTagsRegistrationConfigurator> setupAction)
        {
            var cfg = new CacheTagsRegistrationConfigurator();
            setupAction?.Invoke(cfg);
            services.AddSingleton(cfg);

            services.AddSingleton<ICacheTagWatcher, CacheTagWatcher>();
            services.AddSingleton<IQpContentCacheTagNamingProvider, DefaultQpContentCacheTagNamingProvider>();
            services.AddScoped<QpContentCacheTracker>();
            services.AddScoped<IContentModificationRepository, ContentModificationRepository>();
            services.AddSingleton<ICacheTrackersAccessor, CacheTrackersAccessor>();
            services.AddSingleton<CacheTagsTrackersConfigurator>();

            if (cfg.UseTimer)
            {
                services.AddSingleton<IHostedService, CacheInvalidationBackgroundService>();
            }
        }
    }
}
