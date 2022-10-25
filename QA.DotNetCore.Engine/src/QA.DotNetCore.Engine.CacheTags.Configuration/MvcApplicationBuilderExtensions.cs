using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace QA.DotNetCore.Engine.CacheTags.Configuration
{
    public static class MvcApplicationBuilderExtensions
    {
        /// <summary>
        /// Встраивание в пайплайн инвалидации по кештегам.
        /// </summary>
        public static IApplicationBuilder UseCacheTagsInvalidation(this IApplicationBuilder app)
        {
            var cfg = app.ApplicationServices.GetRequiredService<CacheTagsRegistrationConfigurator>();
            if (cfg.UseMiddleware)
            {
                _ = app.UseMiddleware<CacheInvalidationMiddleware>(cfg.ExcludeRequestPathRegex);
            }

            return app;
        }
    }
}
