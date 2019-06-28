using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using System;
using QP.ConfigurationService.Models;
using Quantumart.QPublishing.Database;

namespace QA.DotNetCore.Engine.AbTesting.Configuration
{
    public class AbTestServicesConfigurator
    {
        public AbTestServicesConfigurator(IServiceCollection services, Action<AbTestOptions> setupAction)
        {
            var options = new AbTestOptions();
            setupAction?.Invoke(options);

            if (options.AbTestingSettings.SiteId == 0)
                throw new Exception("AbTestingSettings.SiteId is not configured.");

            services.AddScoped<IUnitOfWork, UnitOfWork>(sp =>
                {
                    var config = options.QpSettings;
                    DBConnector.ConfigServiceUrl = config.ConfigurationServiceUrl;
                    DBConnector.ConfigServiceToken = config.ConfigurationServiceToken;
                    CustomerConfiguration dbConfig =
                        DBConnector.GetCustomerConfiguration(config.CustomerCode).Result;
                    return new UnitOfWork(dbConfig.ConnectionString, dbConfig.DbType.ToString());

                }
            );
            services.AddScoped<IMetaInfoRepository, MetaInfoRepository>();
            services.AddScoped<INetNameQueryAnalyzer, NetNameQueryAnalyzer>();

            services.AddSingleton(options.AbTestingSettings);
            services.AddScoped<AbTestChoiceResolver>();
            services.AddScoped<IAbTestRepository, AbTestRepository>();
            services.AddScoped<IAbTestService, AbTestService>();
            services.AddSingleton<ICacheProvider, VersionedCacheCoreProvider>();
            services.AddSingleton<IQpContentCacheTagNamingProvider, NullQpContentCacheTagNamingProvider>();
        }
    }
}
