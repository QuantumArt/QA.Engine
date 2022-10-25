using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching.Configuration;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.CacheTags;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Settings;

namespace QA.DotNetCore.Engine.Persistent.Configuration;

public static class PersistentServiceCollectionExtensions
{
    public static void TryAddSiteStructureRepositories(
        this IServiceCollection services,
        Action<QpSiteStructureCacheSettings>? configureSettings = null)
    {
        if (configureSettings is not null)
        {
            _ = services.Configure(configureSettings);
        }

        services.TryAddSingleton((provider) => provider.GetRequiredService<IOptions<QpSiteStructureCacheSettings>>().Value);
        services.TryAddMemoryCacheServices();
        services.TryAddScoped<IMetaInfoRepository, MetaInfoRepository>();
        services.TryAddScoped<INetNameQueryAnalyzer, NetNameQueryAnalyzer>();
        services.TryAddScoped<IQpContentCacheTagNamingProvider, DefaultQpContentCacheTagNamingProvider>();
        services.TryAddScoped<IAbstractItemRepository, AbstractItemRepository>();
        services.TryAddScoped<IItemDefinitionRepository, ItemDefinitionRepository>();
    }
}
