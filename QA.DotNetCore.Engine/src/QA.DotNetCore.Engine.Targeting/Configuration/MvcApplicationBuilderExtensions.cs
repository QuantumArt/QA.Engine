using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;

namespace QA.DotNetCore.Engine.Targeting.Configuration
{
    public static class MvcApplicationBuilderExtensions
    {
        /// <summary>
        /// Регистрируем поставщиков значений таргетирования
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configureTargeting"></param>
        /// <param name="useMiddleware"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseTargeting(this IApplicationBuilder app,
            Action<ServiceSetConfigurator<ITargetingProvider>> configureTargeting, bool useMiddleware=false)
        {
            if (useMiddleware)
            {
                app.UseMiddleware<TargetingMiddleware>();
            }

            var providerConfigurator = app.ApplicationServices.GetRequiredService<ServiceSetConfigurator<ITargetingProvider>>();
            configureTargeting(providerConfigurator);

            return app;
        }


        /// <summary>
        /// Регистрируем поставщиков значений таргетирования
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configureTargeting"></param>
        /// <param name="useMiddleware"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseTargeting(this IApplicationBuilder app,
            Action<ServiceSetConfigurator<ITargetingProviderAsync>> configureTargeting, bool useMiddleware=false)
        {
            if (useMiddleware)
            {
                app.UseMiddleware<TargetingMiddleware>();
            }

            var providerConfigurator = app.ApplicationServices.GetRequiredService<ServiceSetConfigurator<ITargetingProviderAsync>>();
            configureTargeting(providerConfigurator);

            return app;
        }

        /// <summary>
        /// Регистрируем фильтры для таргетирования структуры сайта
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configureFilters"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSiteStructureFilters(this IApplicationBuilder app, Action<KeyedServiceSetConfigurator<TargetingDestination, ITargetingFilter>> configureFilters)
        {
            var builder = app.ApplicationServices.GetRequiredService<KeyedServiceSetConfigurator<TargetingDestination, ITargetingFilter>>();
            configureFilters(builder);
            return app;
        }
    }
}
