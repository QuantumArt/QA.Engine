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
using QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching;
using QA.DotNetCore.Engine.Routing.UrlResolve;
using QA.DotNetCore.Engine.Routing;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using QA.DotNetCore.Engine.Routing.UrlResolve.Targeting;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.QpData.Configuration
{
    public class SiteStructureEngineConfigurator : ISiteStructureEngineConfigurator
    {
        /// <summary>
        /// Конфигурация движка структуры сайта: сервисы для построения структуры сайта
        /// + сервисы необходимые для интеграции структуры сайта с MVC (контроллерами и viewcomponent)
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        public SiteStructureEngineConfigurator(IServiceCollection services, Action<SiteStructureEngineOptions> setupAction)
        {
            Services = services;

            var options = new SiteStructureEngineOptions();
            setupAction?.Invoke(options);

            ConfigureGeneralServices(options);

            services.AddSingleton(new QpSiteStructureCacheSettings
            {
                SiteStructureCachePeriod = options.SiteStructureCachePeriod,
                QpSchemeCachePeriod = options.QpSchemeCachePeriod,
                ItemDefinitionCachePeriod = options.ItemDefinitionCachePeriod
            });

            services.AddSingleton(new UrlTokenConfig
            {
                DefaultTailPattern = options.DefaultUrlTailPattern,
                TailPatternsByControllers = options.UrlTailPatternsByControllers,
                HeadPatterns = options.UrlHeadPatterns ?? new List<HeadUrlMatchingPattern> { new HeadUrlMatchingPattern { Pattern = "/"} }
            });

            services.TryAddScoped<IItemDefinitionRepository, ItemDefinitionRepository>();
            services.TryAddScoped<IAbstractItemFactory, AbstractItemFactory>();
            services.TryAddSingleton<ITargetingFilterAccessor, NullTargetingFilterAccessor>();

            services.TryAddSingleton<ITailUrlResolver, TailUrlResolver>();

            var headTokenPossibleConfigurator = new ServiceSetConfigurator<IHeadTokenPossibleValuesProvider>();
            foreach (var t in options.HeadTokenPossibleValuesProviders)
            {
                headTokenPossibleConfigurator.Register(t);
            }
            services.TryAddSingleton(headTokenPossibleConfigurator);
            services.TryAddSingleton<IHeadTokenPossibleValuesAccessor, HeadTokenPossibleValuesAccessor>();
            services.TryAddSingleton<IHeadUrlResolver, HeadUrlResolver>();

            services.TryAddSingleton<ITargetingUrlTransformator, TargetingUrlTransformator>();
#if NETCOREAPP3_1
            services.TryAddSingleton<SiteStructureRouteValueTransformer>();
#endif
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

            //декорируем дефолтный MVC-ный IViewComponentInvokerFactory собственной реализацией
            //для возможности рендеринга виджетов как viewcomponent
            //вынуждены делать это с использованием reflection, т.к. дефолтная реализация стала internal
            var defaultViewComponentInvokerFactoryType = typeof(IViewComponentInvokerFactory).Assembly.GetType("Microsoft.AspNetCore.Mvc.ViewComponents.DefaultViewComponentInvokerFactory");
            services.AddScoped(defaultViewComponentInvokerFactoryType);
            services.AddScoped<IViewComponentInvokerFactory>(provider =>
                new WidgetViewComponentInvokerFactory((IViewComponentInvokerFactory)provider.GetRequiredService(defaultViewComponentInvokerFactoryType)));
        }

        /// <summary>
        /// Конфигурация сервисов для построения структуры сайта
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        public SiteStructureEngineConfigurator(IServiceCollection services, Action<SiteStructureOptions> setupAction)
        {
            Services = services;

            var options = new SiteStructureOptions();
            setupAction?.Invoke(options);

            ConfigureGeneralServices(options);

            Services.AddSingleton(new QpSiteStructureCacheSettings
            {
                SiteStructureCachePeriod = options.SiteStructureCachePeriod,
                QpSchemeCachePeriod = options.QpSchemeCachePeriod
            });

            Services.TryAddScoped<IAbstractItemFactory, UniversalAbstractItemFactory>();
        }

        public IServiceCollection Services { get; private set; }

        private void ConfigureGeneralServices(SiteStructureOptions options)
        {
            if (options.SiteId == 0)
                throw new Exception("SiteId is not configured.");

            //настройки
            Services.AddSingleton(new QpSiteStructureBuildSettings
            {
                IsStage = options.IsStage,
                SiteId = options.SiteId,
                LoadAbstractItemFieldsToDetailsCollection = options.LoadAbstractItemFieldsToDetailsCollection,
                RootPageDiscriminator = options.RootPageDiscriminator,
                UploadUrlPlaceholder = options.UploadUrlPlaceholder
            });

            //DAL
            if (!Services.Any(x => x.ServiceType == typeof(IUnitOfWork)))
            {
                if (String.IsNullOrWhiteSpace(options.QpConnectionString))
                    throw new Exception("QpConnectionString is not configured.");

                if (String.IsNullOrWhiteSpace(options.QpDatabaseType))
                    throw new Exception("QpDatabaseType is not configured.");

                Services.AddScoped<IUnitOfWork, UnitOfWork>(sp =>
                {
                    return new UnitOfWork(options.QpConnectionString, options.QpDatabaseType);
                });
            }

            Services.TryAddScoped<IMetaInfoRepository, MetaInfoRepository>();
            Services.TryAddScoped<INetNameQueryAnalyzer, NetNameQueryAnalyzer>();
            Services.TryAddScoped<IAbstractItemRepository, AbstractItemRepository>();

            Services.TryAddScoped<IQpUrlResolver, QpUrlResolver>();
            Services.TryAddScoped<IAbstractItemStorageBuilder, QpAbstractItemStorageBuilder>();
            Services.TryAddScoped<IAbstractItemStorageProvider, SimpleAbstractItemStorageProvider>();
            Services.TryAddSingleton<ICacheProvider, VersionedCacheCoreProvider>();
            Services.TryAddSingleton<IQpContentCacheTagNamingProvider, NullQpContentCacheTagNamingProvider>();
            Services.TryAddSingleton<ITargetingFilterAccessor, NullTargetingFilterAccessor>();
            Services.TryAddSingleton<IItemFinder, ItemFinder>();
        }
    }
}
