using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Routing.Mappers;
using QA.DotNetCore.Engine.Widgets;
using QA.DotNetCore.Engine.Widgets.Mappers;
using System;

namespace QA.DotNetCore.Engine.Xml.Configuration
{
    public class XmlSiteStructureEngineConfigurator : ISiteStructureEngineConfigurator
    {
        public XmlSiteStructureEngineConfigurator(IServiceCollection services, Action<XmlSiteStructureEngineOptions> setupAction)
        {
            Services = services;

            var options = new XmlSiteStructureEngineOptions();
            setupAction?.Invoke(options);

            //настройки
            if (options.Settings.FilePath == null)
                throw new Exception("Settings.FilePath is not configured.");

            //itypefinder
            services.Add(new ServiceDescriptor(typeof(ITypeFinder), provider => options.TypeFinder, ServiceLifetime.Singleton));
            services.AddSingleton<XmlAbstractItemFactory>();
            services.AddSingleton<IAbstractItemStorageProvider, XmlAbstractItemStorageProvider>();
            services.AddSingleton(options.Settings);

            if (options.ControllerMapperConvention == ControllerMapperConvention.Name)
                services.AddSingleton<IControllerMapper, NameConventionalControllerMapper>();
            else if (options.ControllerMapperConvention == ControllerMapperConvention.Attribute)
                services.AddSingleton<IControllerMapper, AttributeConventionalControllerMapper>();

            if (options.ComponentMapperConvention == ComponentMapperConvention.Name)
                services.AddSingleton<IComponentMapper, NameConventionalComponentMapper>();
            else if (options.ComponentMapperConvention == ComponentMapperConvention.Attribute)
                services.AddSingleton<IComponentMapper, AttributeConventionalComponentMapper>();
            services.AddScoped<IViewComponentInvokerFactory, WidgetViewComponentInvokerFactory>();
        }

        public IServiceCollection Services { get; private set; }
    }
}
