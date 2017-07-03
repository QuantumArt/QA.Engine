using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;

namespace QA.DotNetCore.Engine.Targeting.Configuration
{
    public static class MvcApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseTargeting(this IApplicationBuilder app, Action<ITargetingProvidersConfigurator> configureTargeting)
        {
            app.UseMiddleware<TargetingMiddleware>();
            var builder = app.ApplicationServices.GetRequiredService<ITargetingProvidersConfigurator>();
            configureTargeting(builder);
            return app;
        }

        public static IApplicationBuilder UseSiteSctructureFilters(this IApplicationBuilder app, Action<ITargetingFiltersConfigurator> configureFilters)
        {
            var builder = app.ApplicationServices.GetRequiredService<ITargetingFiltersConfigurator>();
            configureFilters(builder);
            return app;
        }
    }
}
