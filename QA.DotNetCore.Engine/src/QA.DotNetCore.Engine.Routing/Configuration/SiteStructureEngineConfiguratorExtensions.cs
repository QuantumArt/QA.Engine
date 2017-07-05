using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Routing.Mappers;

namespace QA.DotNetCore.Engine.Routing.Configuration
{
    public static class SiteStructureEngineConfiguratorExtensions
    {
        /// <summary>
        /// Зарегистрировать маппер контроллеров с указанной конвенцией
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="convention"></param>
        /// <returns></returns>
        public static ISiteStructureEngineConfigurator AddControllerMapper(this ISiteStructureEngineConfigurator cfg, ControllerMapperConvention convention)
        {
            if (convention == ControllerMapperConvention.Name)
                cfg.Services.AddSingleton<IControllerMapper, NameConventionalControllerMapper>();
            else if (convention == ControllerMapperConvention.Attribute)
                cfg.Services.AddSingleton<IControllerMapper, AttributeConventionalControllerMapper>();

            return cfg;
        }

        
    }

    /// <summary>
    /// Конвенция о способе маппинга страниц структуры сайта и контроллеров MVC
    /// </summary>
    public enum ControllerMapperConvention
    {
        /// <summary>
        /// Конвенция предполагает, что контроллер должен называться также как тип страницы
        /// </summary>
        Name,
        /// <summary>
        /// Конвенция предполагает, что контроллер должен быть помечен атрибутом, в котором должен быть указан тип страницы
        /// </summary>
        Attribute
    }
}
