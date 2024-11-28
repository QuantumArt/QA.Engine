using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Finder;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.CacheTags.Configuration;
using QA.DotNetCore.Engine.Persistent.Configuration;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.Engine.Routing;
using QA.DotNetCore.Engine.Routing.Mappers;
using QA.DotNetCore.Engine.Routing.UrlResolve;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching;
using QA.DotNetCore.Engine.Routing.UrlResolve.Targeting;
using QA.DotNetCore.Engine.Targeting.TargetingProviders;
using QA.DotNetCore.Engine.Widgets;
using QA.DotNetCore.Engine.Widgets.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

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

            services.AddSingleton(new UrlTokenConfig
            {
                DefaultTailPattern = options.DefaultUrlTailPattern,
                TailPatternsByControllers = options.UrlTailPatternsByControllers,
                HeadPatterns = options.UrlHeadPatterns ?? new List<HeadUrlMatchingPattern> { new HeadUrlMatchingPattern { Pattern = "/" } }
            });

           services.AddSingleton(options.DictionarySettings ?? new List<DictionarySettings>());

            services.TryAddScoped<IAbstractItemFactory, AbstractItemFactory>();
            services.TryAddSingleton<ITargetingFilterAccessor, NullTargetingFilterAccessor>();
            services.TryAddSingleton<ITargetingContext, NullTargetingContext>();

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
            services.TryAddSingleton<UrlTokenTargetingProvider>();
            services.TryAddScoped<RegionTargetingProvider>();
            services.TryAddScoped<CultureTargetingProvider>();
            services.TryAddScoped<IDictionaryProvider, DictionaryProvider>();

            services.TryAddSingleton<SiteStructureRouteValueTransformer>();
            services.TryAdd(new ServiceDescriptor(typeof(ITypeFinder), provider => options.TypeFinder, ServiceLifetime.Singleton));

            if (options.ItemDefinitionConvention == ItemDefinitionConvention.Name)
            {
                services.TryAddScoped<IItemDefinitionProvider, NameConventionalItemDefinitionProvider>();
            }
            else if (options.ItemDefinitionConvention == ItemDefinitionConvention.Attribute)
            {
                throw new NotImplementedException("AttributeConventionalItemDefinitionProvider not implemented yet");
            }

            if (options.ControllerMapperConvention == ControllerMapperConvention.Name)
            {
                services.TryAddSingleton<IControllerMapper, NameConventionalControllerMapper>();
            }
            else if (options.ControllerMapperConvention == ControllerMapperConvention.Attribute)
            {
                services.TryAddSingleton<IControllerMapper, AttributeConventionalControllerMapper>();
            }

            if (options.ComponentMapperConvention == ComponentMapperConvention.Name)
            {
                services.TryAddSingleton<IComponentMapper, NameConventionalComponentMapper>();
            }
            else if (options.ComponentMapperConvention == ComponentMapperConvention.Attribute)
            {
                services.TryAddSingleton<IComponentMapper, AttributeConventionalComponentMapper>();
            }

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

            Services.TryAddScoped<IAbstractItemFactory, UniversalAbstractItemFactory>();
        }

        public IServiceCollection Services { get; private set; }

        private void ConfigureGeneralServices(SiteStructureOptions options)
        {
            if (options.SiteId == 0)
            {
                throw new Exception("SiteId is not configured.");
            }

            //настройки
            Services.AddSingleton(new QpSiteStructureBuildSettings
            {
                IsStage = options.IsStage,
                SiteId = options.SiteId,
                LoadAbstractItemFieldsToDetailsCollection = options.LoadAbstractItemFieldsToDetailsCollection,
                RootPageDiscriminator = options.RootPageDiscriminator,
                UploadUrlPlaceholder = options.UploadUrlPlaceholder,
                CacheFetchTimeoutAbstractItemStorage = options.CacheFetchTimeoutAbstractItemStorage
            });

            //DAL
            TryAddUnitOfWork(options);

            _ = Services.Configure<QpSiteStructureCacheSettings>(settings =>
            {
                settings.SiteStructureCachePeriod = options.SiteStructureCachePeriod;
                settings.QpSchemeCachePeriod = options.QpSchemeCachePeriod;
                settings.ItemDefinitionCachePeriod = options.ItemDefinitionCachePeriod;
                settings.SiteStructureCachingType = options.SiteStructureCachingType;
            });

            _ = Services.AddCacheTagServices();
            Services.TryAddSiteStructureRepositories();

            Services.TryAddScoped<IQpUrlResolver, QpUrlResolver>();
            Services.TryAddTransient<IAbstractItemStorageBuilder, QpAbstractItemStorageBuilder>();
            Services.TryAddTransient<IAbstractItemContextStorageBuilder, QpAbstractItemStorageBuilder>();

            if (options.SiteStructureCachePeriod <= TimeSpan.Zero || options.SiteStructureCachingType == SiteStructureCachingType.None)
            {
                Services.TryAddScoped<IAbstractItemStorageProvider, AbstractItemStorageProvider>();
            }
            else if (options.SiteStructureCachingType == SiteStructureCachingType.Granular)
            {
                Services.TryAddScoped<IAbstractItemStorageProvider, GranularCacheAbstractItemStorageProvider>();
            }
            else
            {
                Services.TryAddScoped<IAbstractItemStorageProvider, SimpleCacheAbstractItemStorageProvider>();
            }

            Services.TryAddSingleton<ITargetingFilterAccessor, NullTargetingFilterAccessor>();
            Services.TryAddSingleton<IItemFinder, ItemFinder>();
        }

        private void TryAddUnitOfWork(SiteStructureOptions options)
        {
            if (!Services.Any(x => x.ServiceType == typeof(IUnitOfWork)))
            {
                if (String.IsNullOrWhiteSpace(options.QpConnectionString))
                {
                    throw new Exception("QpConnectionString is not configured.");
                }

                if (String.IsNullOrWhiteSpace(options.QpDatabaseType))
                {
                    throw new Exception("QpDatabaseType is not configured.");
                }

                Services.AddScoped<IUnitOfWork, UnitOfWork>(sp => new UnitOfWork(
                    options.QpConnectionString,
                    options.QpDatabaseType)
                );

                Services.AddScoped<Func<IUnitOfWork>>(sp => sp.GetRequiredService<IUnitOfWork>);
                Services.AddScoped<Func<IServiceProvider, IUnitOfWork>>(_ => (provider) => provider.GetRequiredService<IUnitOfWork>());

            }
        }
    }
}
