using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching;
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

            string qpConnectionString = null;
            string qpDatabaseType = null;
            if (options.QpSettings?.ConnectionString != null)
            {
                //если явно задана QpConnectionString, то будем использовать эту строку подключения
                qpConnectionString = options.QpSettings.ConnectionString;
                qpDatabaseType = options.QpSettings.DatabaseType ?? "MSSQL";
            }
            else if (options.QpSettings?.CustomerCode != null && options.QpSettings?.ConfigurationServiceUrl != null && options.QpSettings?.ConfigurationServiceToken != null)
            {
                //если есть возможность получить строку подключения через сервис конфигурации
                DBConnector.ConfigServiceUrl = options.QpSettings.ConfigurationServiceUrl;
                DBConnector.ConfigServiceToken = options.QpSettings.ConfigurationServiceToken;
                CustomerConfiguration dbConfig = DBConnector.GetCustomerConfiguration(options.QpSettings.CustomerCode).Result;

                qpConnectionString = dbConfig.ConnectionString;
                qpDatabaseType = dbConfig.DbType.ToString();
            }
            else
            {
                throw new Exception("Cannot get QP connection details. Should provide ConnectionString/DatabaseType or ConfigurationServiceUrl/ConfigurationServiceToken/CustomerCode in QpSettings");
            }

            services.AddScoped<IUnitOfWork, UnitOfWork>(sp =>
            {
                return new UnitOfWork(qpConnectionString, qpDatabaseType);
            });
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
