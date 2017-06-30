using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.QpData
{
    public static class MvcServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление сервисов для движка структуры сайта в IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        public static ISiteStructureEngineConfigurator AddSiteStructureEngine(this IServiceCollection services, IConfigurationRoot cfg)
        {
            return new SiteStructureEngineConfigurator(services, cfg);
        }
    }
}
