using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using System;

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

            services.AddScoped<IUnitOfWork, UnitOfWork>(sp => new UnitOfWork(options.QpConnectionString));
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
