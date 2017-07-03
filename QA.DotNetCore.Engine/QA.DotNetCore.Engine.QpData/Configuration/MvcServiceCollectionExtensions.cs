using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using System;

namespace QA.DotNetCore.Engine.QpData.Configuration
{
    public static class MvcServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление сервисов для движка структуры сайта в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        public static ISiteStructureEngineConfigurator AddSiteStructureEngine(this IServiceCollection services)
        {
            return AddSiteStructureEngine(services, null);
        }

        /// <summary>
        /// Добавление сервисов для движка структуры сайта в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        /// <param name="setupAction">действие для изменения предзаданных параметров структуры сайта</param>
        public static ISiteStructureEngineConfigurator AddSiteStructureEngine(this IServiceCollection services, Action<SiteStructureEngineOptions> setupAction)
        {
            return new SiteStructureEngineConfigurator(services, setupAction);
        }
    }
}
