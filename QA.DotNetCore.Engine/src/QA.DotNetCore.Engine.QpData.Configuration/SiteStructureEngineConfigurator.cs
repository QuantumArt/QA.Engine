using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.Engine.Routing.Mappers;
using QA.DotNetCore.Engine.Widgets;
using QA.DotNetCore.Engine.Widgets.Mappers;
using System;
using System.Linq;
using QA.DotNetCore.Engine.QpData.Settings;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QA.DotNetCore.Engine.Abstractions.Finder;

namespace QA.DotNetCore.Engine.QpData.Configuration
{
    public class SiteStructureEngineConfigurator : ISiteStructureEngineConfigurator
    {
        public SiteStructureEngineConfigurator(IServiceCollection services, Action<SiteStructureEngineOptions> setupAction)
        {
            Services = services;

            var options = new SiteStructureEngineOptions();
            setupAction?.Invoke(options);

            if (options.SiteId == 0)
                throw new Exception("SiteId is not configured.");

            //настройки
            services.AddSingleton(new QpSiteStructureBuildSettings
            {
                IsStage = options.IsStage,
                SiteId = options.SiteId,
                LoadAbstractItemFieldsToDetailsCollection = options.LoadAbstractItemFieldsToDetailsCollection,
                LoadM2mForAbstractItem = options.LoadM2mForAbstractItem,
                LoadM2mForAllExtensions = options.LoadM2mForAllExtensions,
                RootPageDiscriminator = options.RootPageDiscriminator,
                UploadUrlPlaceholder = options.UploadUrlPlaceholder
            });

            services.AddSingleton(new QpSiteStructureCacheSettings
            {
                SiteStructureCachePeriod = options.SiteStructureCachePeriod,
                QpSchemeCachePeriod = options.QpSchemeCachePeriod,
                ItemDefinitionCachePeriod = options.ItemDefinitionCachePeriod
            });

            //DAL
            if (!services.Any(x => x.ServiceType == typeof(IUnitOfWork)))
            {
                if (String.IsNullOrWhiteSpace(options.QpConnectionString))
                    throw new Exception("QpConnectionString is not configured.");

                if (String.IsNullOrWhiteSpace(options.QpDatabaseType))
                    throw new Exception("QpDatabaseType is not configured.");

                services.AddScoped<IUnitOfWork, UnitOfWork>(sp =>
                {
                    return new UnitOfWork(options.QpConnectionString, options.QpDatabaseType);
                });
            }
            services.TryAddScoped<IMetaInfoRepository, MetaInfoRepository>();
            services.TryAddScoped<INetNameQueryAnalyzer, NetNameQueryAnalyzer>();
            services.TryAddScoped<IAbstractItemRepository, AbstractItemRepository>();
            services.TryAddScoped<IItemDefinitionRepository, ItemDefinitionRepository>();

            //сервисы
            services.TryAddScoped<IAbstractItemFactory, AbstractItemFactory>();
            services.TryAddScoped<IQpUrlResolver, QpUrlResolver>();
            services.TryAddScoped<IAbstractItemStorageBuilder, QpAbstractItemStorageBuilder>();
            services.TryAddScoped<IAbstractItemStorageProvider, SimpleAbstractItemStorageProvider>();
            services.TryAddSingleton<ICacheProvider, VersionedCacheCoreProvider>();
            services.TryAddSingleton<IQpContentCacheTagNamingProvider, NullQpContentCacheTagNamingProvider>();
            services.TryAddSingleton<IItemFinder, ItemFinder>();

            //itypefinder
            services.TryAdd(new ServiceDescriptor(typeof(ITypeFinder), provider => options.TypeFinder, ServiceLifetime.Singleton));

            if (options.ItemDefinitionConvention == ItemDefinitionConvention.Name)
                services.TryAddScoped<IItemDefinitionProvider, NameConventionalItemDefinitionProvider>();
            else if (options.ItemDefinitionConvention == ItemDefinitionConvention.Attribute)
                throw new NotImplementedException("AttributeConventionalItemDefinitionProvider not implemented yet");

            if (options.ControllerMapperConvention == ControllerMapperConvention.Name)
                services.TryAddSingleton<IControllerMapper, NameConventionalControllerMapper>();
            else if (options.ControllerMapperConvention == ControllerMapperConvention.Attribute)
                services.TryAddSingleton<IControllerMapper, AttributeConventionalControllerMapper>();

            if (options.ComponentMapperConvention == ComponentMapperConvention.Name)
                services.TryAddSingleton<IComponentMapper, NameConventionalComponentMapper>();
            else if (options.ComponentMapperConvention == ComponentMapperConvention.Attribute)
                services.TryAddSingleton<IComponentMapper, AttributeConventionalComponentMapper>();

            //заменяем дефолтный MVC-ный IViewComponentInvokerFactory на собственную реализацию
            //для возможности рендеринга виджетов как viewcomponent
            services.AddScoped<IViewComponentInvokerFactory, WidgetViewComponentInvokerFactory>();
        }

        public IServiceCollection Services { get; private set; }
    }
}
