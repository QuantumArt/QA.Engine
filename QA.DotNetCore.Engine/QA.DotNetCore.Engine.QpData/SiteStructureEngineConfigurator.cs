using QA.DotNetCore.Engine.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Replacements;
using System;

namespace QA.DotNetCore.Engine.QpData
{
    public class SiteStructureEngineConfigurator : ISiteStructureEngineConfigurator
    {
        public SiteStructureEngineConfigurator(IServiceCollection services, IConfigurationRoot cfg)
        {
            Services = services;

            //настройки
            services.Configure<QpSiteStructureSettings>(cfg.GetSection("QpSiteStructureSettings"));
            services.Configure<QpSettings>(cfg.GetSection("QpSettings"));
            services.Configure<SiteMode>(cfg.GetSection("SiteMode"));

            //DAL
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAbstractItemRepository, AbstractItemRepository>();
            services.AddScoped<IMetaInfoRepository, MetaInfoRepository>();
            services.AddScoped<IItemDefinitionRepository, ItemDefinitionRepository>();

            //сервисы
            services.AddScoped<IAbstractItemFactory, AbstractItemFactory>();
            services.AddScoped<IQpUrlResolver, QpUrlResolver>();
            services.AddScoped<IAbstractItemStorageBuilder, QpAbstractItemStorageBuilder>();
            services.AddScoped<IAbstractItemStorageProvider, SimpleAbstractItemStorageProvider>();

        }

        public IServiceCollection Services { get; private set; }
    }

    public static class SiteStructureEngineConfiguratorExtensions
    {
        public static ISiteStructureEngineConfigurator AddItemDefinitionProvider(this ISiteStructureEngineConfigurator cfg, ItemDefinitionConvention convention)
        {
            if (convention == ItemDefinitionConvention.Name)
                cfg.Services.AddScoped<IItemDefinitionProvider, NameConventionalItemDefinitionProvider>();
            else if (convention == ItemDefinitionConvention.Attribute)
                throw new NotImplementedException("AttributeConventionalItemDefinitionProvider not implemented yet");

            return cfg;
        }

        public enum ItemDefinitionConvention
        {
            Name,
            Attribute
        }
    }
}
