using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
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
        [Obsolete(
            "Use " + nameof(UseCacheTagsInvalidation) + " overload without arguments "
            + "and configure trackers using " + nameof(MvcServiceCollectionExtensions.AddCacheTagServices)
            + " method instead.")]
        public static IApplicationBuilder UseCacheTagsInvalidation(this IApplicationBuilder app, Action<ServiceSetConfigurator<ICacheTagTracker>> configureTrackers)
        {
            var cfgTrackers = app.ApplicationServices.GetRequiredService<ServiceSetConfigurator<ICacheTagTracker>>();

            //наполняем CacheTagsTrackersConfigurator трекерами, которые были настроены
            configureTrackers?.Invoke(cfgTrackers);

            var cfg = app.ApplicationServices.GetRequiredService<CacheTagsRegistrationConfigurator>();
            if (cfg.UseMiddleware)
            {
                app.UseMiddleware<CacheInvalidationMiddleware>(cfg.ExcludeRequestPathRegex);
            }

            return app;
        }

        /// <summary>
        /// Встраивание в пайплайн инвалидации по кештегам.
        /// </summary>
        public static IApplicationBuilder UseCacheTagsInvalidation(this IApplicationBuilder app) =>
#pragma warning disable CS0618 // Type or member is obsolete
            app.UseCacheTagsInvalidation(null);
#pragma warning restore CS0618 // Type or member is obsolete
    }
}
