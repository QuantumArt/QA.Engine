using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Caching.Utils.Configuration
{
    public static class MvcApplicationBuilderExtensions
    {
        /// <summary>
        /// Добавляем в application компоненты для инвалидации по кештегам
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configureInvalidation"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCacheTagsInvalidation(this IApplicationBuilder app, Action<CacheTagsInvalidationConfigurator> configureInvalidation)
        {
            var cfg = app.ApplicationServices.GetRequiredService<CacheTagsInvalidationConfigurator>();
            configureInvalidation(cfg);

            if (cfg.UseMiddleware)
            {
                app.UseMiddleware<CacheInvalidationMiddleware>(cfg.ExcludeRequestPathRegex);
            }
            
            return app;
        }
    }
}
