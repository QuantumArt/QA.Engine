using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using System;

namespace QA.DotNetCore.Engine.Xml.Configuration
{
    public static class MvcServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление сервисов для xml-движка структуры сайта в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        public static ISiteStructureEngineConfigurator AddSiteStructureEngineViaXml(this IServiceCollection services)
        {
            return AddSiteStructureEngineViaXml(services, null);
        }

        /// <summary>
        /// Добавление сервисов для xml-движка структуры сайта в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        /// <param name="setupAction">действие для изменения предзаданных параметров структуры сайта</param>
        public static ISiteStructureEngineConfigurator AddSiteStructureEngineViaXml(this IServiceCollection services, Action<XmlSiteStructureEngineOptions> setupAction)
        {
            return new XmlSiteStructureEngineConfigurator(services, setupAction);
        }
    }
}
