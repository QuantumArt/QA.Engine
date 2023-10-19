using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Targeting.Filters;

namespace QA.DotNetCore.Engine.Targeting.Configuration
{
    public static class MvcServiceCollectionExtensions
    {
        /// <summary>
        /// Когда функционал таргетирования не реализуется
        /// </summary>
        /// <param name="services"></param>
        public static void AddNullTargeting(this IServiceCollection services)
        {
            services.AddSingleton<ITargetingFilterAccessor, NullTargetingFilterAccessor>();
            services.AddSingleton<ITargetingContextUpdater, NullTargetingContextUpdater>();
        }

        /// <summary>
        /// Добавление сервисов для таргетирования в IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        public static void AddTargeting(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddSingleton<ITargetingContextUpdater, HttpTargetingContextUpdater>();
            services.AddSingleton<ITargetingContext, HttpTargetingContext>();
            services.AddSingleton<ServiceSetConfigurator<ITargetingProvider>>();
            services.AddSingleton<ServiceSetConfigurator<ITargetingProviderAsync>>();

            services.AddSingleton<ITargetingFilterAccessor, TargetingFilterAccessor>();
            services.AddSingleton<KeyedServiceSetConfigurator<TargetingDestination, ITargetingFilter>>();
        }
    }
}
