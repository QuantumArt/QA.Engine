using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Routing.Mappers;

namespace QA.DotNetCore.Engine.Routing.Configuration
{
    public static class SiteStructureEngineConfiguratorExtensions
    {
        public static ISiteStructureEngineConfigurator AddControllerMapper(this ISiteStructureEngineConfigurator cfg, ControllerMapperConvention convention)
        {
            if (convention == ControllerMapperConvention.Name)
                cfg.Services.AddSingleton<IControllerMapper, NameConventionalControllerMapper>();
            else if (convention == ControllerMapperConvention.Attribute)
                cfg.Services.AddSingleton<IControllerMapper, AttributeConventionalControllerMapper>();

            return cfg;
        }

        public enum ControllerMapperConvention
        {
            Name,
            Attribute
        }
    }
}
