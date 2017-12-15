using Microsoft.Extensions.DependencyInjection;
using System;

namespace QA.DotNetCore.Engine.Widgets.Configuration
{
    public static class MvcServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление сервисов для движка структуры сайта в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        public static OnScreenConfigurator AddOnScreenServices(this IServiceCollection services)
        {
            return AddOnScreenServices(services, null);
        }

        /// <summary>
        /// Добавление сервисов для движка структуры сайта в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        /// <param name="setupAction">действие для изменения предзаданных параметров структуры сайта</param>
        public static OnScreenConfigurator AddOnScreenServices(this IServiceCollection services, Action<OnScreenSettings> setupAction)
        {
            return new OnScreenConfigurator(services, setupAction);
        }
    }
}
