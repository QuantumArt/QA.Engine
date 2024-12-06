using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Persistent.Configuration;
using QA.DotNetCore.Engine.CacheTags.Configuration;

namespace QA.DotNetCore.Engine.AbTesting.Configuration
{
    public class AbTestServicesConfigurator
    {
        public AbTestServicesConfigurator(IServiceCollection services, Action<AbTestOptions> setupAction)
        {
            var options = new AbTestOptions();
            setupAction?.Invoke(options);

            //настройки
            if (options.SiteId == 0)
                throw new Exception("AbTestingSettings.SiteId is not configured.");

            services.AddSingleton(new AbTestingQpSettings
            {
                IsStage = options.IsStage,
                SiteId = options.SiteId
            });
            services.AddSingleton(new AbTestingCacheSettings
            {
                TestsCachePeriod = options.TestsCachePeriod,
                ContainersCachePeriod = options.ContainersCachePeriod
            });

            //DAL
            if (!services.Any(x => x.ServiceType == typeof(IUnitOfWork)))
            {
                if (String.IsNullOrWhiteSpace(options.QpConnectionString))
                    throw new Exception("QpConnectionString is not configured.");

                if (String.IsNullOrWhiteSpace(options.QpDatabaseType))
                    throw new Exception("QpDatabaseType is not configured.");

                services.AddScoped<IUnitOfWork>(sp => new UnitOfWork(
                    options.QpConnectionString,
                    options.QpDatabaseType,
                    sp.GetRequiredService<ILogger<UnitOfWork>>()));
            }

            services.TryAddSiteStructureRepositories();
            _ = services.AddCacheTagServices();

            //сервисы
            services.TryAddScoped<AbTestChoiceResolver>();
            services.TryAddScoped<IAbTestRepository, AbTestRepository>();
            services.TryAddScoped<IAbTestService, AbTestService>();
        }
    }
}
