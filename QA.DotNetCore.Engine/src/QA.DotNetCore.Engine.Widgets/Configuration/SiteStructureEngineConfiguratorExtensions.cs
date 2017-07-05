using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Widgets.Mappers;

namespace QA.DotNetCore.Engine.Widgets.Configuration
{
    public static class SiteStructureEngineConfiguratorExtensions
    {
        /// <summary>
        /// Зарегистрировать IViewComponentInvokerFactory
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static ISiteStructureEngineConfigurator AddWidgetInvokerFactory(this ISiteStructureEngineConfigurator cfg)
        {
            cfg.Services.AddScoped<IViewComponentInvokerFactory, WidgetViewComponentInvokerFactory>();
            
            return cfg;
        }

        /// <summary>
        /// Зарегистрировать маппер ViewComponent с указанной конвенцией
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="convention"></param>
        /// <returns></returns>
        public static ISiteStructureEngineConfigurator AddComponentMapper(this ISiteStructureEngineConfigurator cfg, ComponentMapperConvention convention)
        {
            if (convention == ComponentMapperConvention.Name)
                cfg.Services.AddSingleton<IComponentMapper, NameConventionalComponentMapper>();
            else if (convention == ComponentMapperConvention.Attribute)
                cfg.Services.AddSingleton<IComponentMapper, AttributeConventionalComponentMapper>();

            return cfg;
        }

        
    }

    /// <summary>
    /// Конвенция о способе маппинга виджетов структуры сайта и классов ViewComponent
    /// </summary>
    public enum ComponentMapperConvention
    {
        /// <summary>
        /// Конвенция предполагает, что компонент должен называться также как тип виджета
        /// </summary>
        Name,
        /// <summary>
        /// Конвенция предполагает, что компонент должен быть помечен атрибутом, в котором должен быть указан тип виджета
        /// </summary>
        Attribute
    }
}
