using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.Engine.Routing.Mappers;
using QA.DotNetCore.Engine.Widgets;
using QA.DotNetCore.Engine.Widgets.Mappers;
using System;

namespace QA.DotNetCore.Engine.QpData.Configuration
{
    public class SiteStructureEngineConfigurator : ISiteStructureEngineConfigurator
    {
        public SiteStructureEngineConfigurator(IServiceCollection services, Action<SiteStructureEngineOptions> setupAction)
        {
            Services = services;

            var options = new SiteStructureEngineOptions();
            setupAction?.Invoke(options);

            //настройки
            if (options.QpSettings == null)
                throw new Exception("QpSettings is not configured.");

            if (options.QpConnectionString == null)
                throw new Exception("QpConnectionString is not configured.");

            services.AddSingleton(options.QpSettings);
            services.AddSingleton(options.QpSiteStructureSettings);
            services.AddSingleton(options.QpSchemeCacheSettings);
            services.AddSingleton(options.ItemDefinitionCacheSettings);

            //DAL
            services.AddScoped<IUnitOfWork, UnitOfWork>(sp => new UnitOfWork(options.QpConnectionString));
            services.AddScoped<IMetaInfoRepository, MetaInfoRepository>();
            services.AddScoped<INetNameQueryAnalyzer, NetNameQueryAnalyzer>();
            services.AddScoped<IAbstractItemRepository, AbstractItemRepository>();
            services.AddScoped<IItemDefinitionRepository, ItemDefinitionRepository>();

            //сервисы
            services.AddScoped<IAbstractItemFactory, AbstractItemFactory>();
            services.AddScoped<IQpUrlResolver, QpUrlResolver>();
            services.AddScoped<IAbstractItemStorageBuilder, QpAbstractItemStorageBuilder>();
            services.AddScoped<IAbstractItemStorageProvider, SimpleAbstractItemStorageProvider>();

            //itypefinder
            services.Add(new ServiceDescriptor(typeof(ITypeFinder), provider => options.TypeFinder, ServiceLifetime.Singleton));
            
            if (options.ItemDefinitionConvention == ItemDefinitionConvention.Name)
                services.AddScoped<IItemDefinitionProvider, NameConventionalItemDefinitionProvider>();
            else if (options.ItemDefinitionConvention == ItemDefinitionConvention.Attribute)
                throw new NotImplementedException("AttributeConventionalItemDefinitionProvider not implemented yet");

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
