using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace QA.DotNetCore.Engine.CacheTags.Configuration
{
    public static class MvcApplicationBuilderExtensions
    {
        /// <summary>
        /// Добавляем в application компоненты для инвалидации по кештегам
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configureTrackers"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCacheTagsInvalidation(this IApplicationBuilder app, Action<CacheTagsTrackersConfigurator> configureTrackers)
        {
            var cfgTrackers = app.ApplicationServices.GetRequiredService<CacheTagsTrackersConfigurator>();
            configureTrackers(cfgTrackers);//наполняем CacheTagsTrackersConfigurator трекерами, которые были настроены

            var cfg = app.ApplicationServices.GetRequiredService<CacheTagsRegistrationConfigurator>();
            if (cfg.UseMiddleware)
            {
                app.UseMiddleware<CacheInvalidationMiddleware>(cfg.ExcludeRequestPathRegex);
            }
            
            return app;
        }
    }
}
