using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Targeting.Filters;

namespace QA.DotNetCore.Engine.Targeting.Configuration
{
    public static class MvcServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление сервисов для таргетирования в IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        public static void AddTargeting(this IServiceCollection services)
        {
            services.AddSingleton<ITargetingContext, HttpTargetingContext>();
            services.AddSingleton<ServiceSetConfigurator<ITargetingProvider>>();
            services.AddSingleton<ServiceSetConfigurator<ITargetingPossibleValuesProvider>>();

            services.AddSingleton<ITargetingFilterAccessor, TargetingFilterAccessor>();
            services.AddSingleton<ServiceSetConfigurator<ITargetingFilter>>();
        }
    }
}
