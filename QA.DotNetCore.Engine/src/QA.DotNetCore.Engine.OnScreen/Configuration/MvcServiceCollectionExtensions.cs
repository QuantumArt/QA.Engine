using Microsoft.Extensions.DependencyInjection;
using System;

namespace QA.DotNetCore.Engine.OnScreen.Configuration
{
    public static class MvcServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление сервисов для работы onscreen в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        /// <param name="mvcBuilder"></param>
        public static OnScreenConfigurator AddOnScreenIntegration(this IServiceCollection services, IMvcBuilder mvcBuilder)
        {
            return AddOnScreenIntegration(services, mvcBuilder, null);
        }

        /// <summary>
        /// Добавление сервисов для работы onscreen в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        /// <param name="mvcBuilder"></param>
        /// <param name="setupAction">действие для изменения предзаданных параметров интеграции с OnScreen</param>
        public static OnScreenConfigurator AddOnScreenIntegration(this IServiceCollection services, IMvcBuilder mvcBuilder, Action<OnScreenConfigurationOptions> setupAction)
        {
            return new OnScreenConfigurator(services, mvcBuilder, setupAction);
        }
    }
}
