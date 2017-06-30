using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Widgets.Mappers;

namespace QA.DotNetCore.Engine.Widgets
{
    public static class SiteStructureEngineConfiguratorExtensions
    {
        public static ISiteStructureEngineConfigurator AddWidgetInvokerFactory(this ISiteStructureEngineConfigurator cfg)
        {
            cfg.Services.AddScoped<IViewComponentInvokerFactory, WidgetViewComponentInvokerFactory>();
            
            return cfg;
        }

        public static ISiteStructureEngineConfigurator AddComponentMapper(this ISiteStructureEngineConfigurator cfg, ComponentMapperConvention convention)
        {
            if (convention == ComponentMapperConvention.Name)
                cfg.Services.AddSingleton<IComponentMapper, NameConventionalComponentMapper>();
            else if (convention == ComponentMapperConvention.Attribute)
                cfg.Services.AddSingleton<IComponentMapper, AttributeConventionalComponentMapper>();

            return cfg;
        }

        public enum ComponentMapperConvention
        {
            Name,
            Attribute
        }
    }
}
