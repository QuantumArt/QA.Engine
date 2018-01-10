using Microsoft.Extensions.DependencyInjection;
using System;

namespace QA.DotNetCore.Engine.OnScreen.Configuration
{
    public static class MvcServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление сервисов для движка структуры сайта в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        public static OnScreenConfigurator AddOnScreenIntegration(this IServiceCollection services)
        {
            return AddOnScreenIntegration(services, null);
        }

        /// <summary>
        /// Добавление сервисов для движка структуры сайта в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        /// <param name="setupAction">действие для изменения предзаданных параметров интеграции с OnScreen</param>
        public static OnScreenConfigurator AddOnScreenIntegration(this IServiceCollection services, Action<OnScreenConfigurationOptions> setupAction)
        {
            return new OnScreenConfigurator(services, setupAction);
        }
    }
}
