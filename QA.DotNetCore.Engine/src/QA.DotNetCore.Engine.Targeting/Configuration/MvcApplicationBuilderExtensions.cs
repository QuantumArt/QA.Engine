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
        /// Регистрируем поставщиков значений таргетирования и поставщиков возможных значений таргетирования
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configureTargeting"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseTargeting(this IApplicationBuilder app, Action<ServiceSetConfigurator<ITargetingProvider>, ServiceSetConfigurator<ITargetingPossibleValuesProvider>> configureTargeting)
        {
            app.UseMiddleware<TargetingPossibleValuesMiddleware>();
            app.UseMiddleware<TargetingMiddleware>();
            var providerConfigurator = app.ApplicationServices.GetRequiredService<ServiceSetConfigurator<ITargetingProvider>>();
            var possibleValuesConfigurator = app.ApplicationServices.GetRequiredService<ServiceSetConfigurator<ITargetingPossibleValuesProvider>>();
            configureTargeting(providerConfigurator, possibleValuesConfigurator);
            return app;
        }

        /// <summary>
        /// Регистрируем фильтры для таргетирования структуры сайта
        /// </summary>
        /// <param name="app"></param>
        /// <param name="configureFilters"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseSiteSctructureFilters(this IApplicationBuilder app, Action<ServiceSetConfigurator<ITargetingFilter>> configureFilters)
        {
            var builder = app.ApplicationServices.GetRequiredService<ServiceSetConfigurator<ITargetingFilter>>();
            configureFilters(builder);
            return app;
        }
    }
}
