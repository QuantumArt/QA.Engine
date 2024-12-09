using Dapper;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Engine.Persistent.Interfaces.Logging;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class ItemDefinitionRepository : IItemDefinitionRepository
    {
        private readonly ILogger _logger;
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        private readonly ICacheProvider _cacheProvider;
        private readonly QpSiteStructureCacheSettings _cacheSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly INetNameQueryAnalyzer _netNameQueryAnalyzer;
        private readonly string[] _potentiallyMissingColumns = new[] { "QPDiscriminator.FrontModuleUrl", "QPDiscriminator.ModuleName" };

        public ItemDefinitionRepository(
            IServiceProvider serviceProvider,
            INetNameQueryAnalyzer netNameQueryAnalyzer,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            ICacheProvider cacheProvider,
            QpSiteStructureCacheSettings cacheSettings,
            ILogger<ItemDefinitionRepository> logger)
        {
            _serviceProvider = serviceProvider;
            _netNameQueryAnalyzer = netNameQueryAnalyzer;
            _cacheSettings = cacheSettings;
            _cacheProvider = cacheProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _logger = logger;
        }

        protected IUnitOfWork UnitOfWork
        {
            get
            {
                var uow = _serviceProvider.GetRequiredService<IUnitOfWork>();
                using var _ = _logger.BeginScopeWith(("unitOfWorkId", uow.Id));
                _logger.LogTrace("Received UnitOfWork from ServiceProvider");
                return uow;
            }
        }

        private const string CmdGetAll = @"
SELECT
    CONTENT_ITEM_ID as Id,
    |QPDiscriminator.Name| as Discriminator,
    |QPDiscriminator.TypeName| as TypeName,
    |QPDiscriminator.IsPage| as IsPage,
    |QPDiscriminator.Title| as Title,
    |QPDiscriminator.Description| as Description,
    |QPDiscriminator.IconUrl| as IconUrl,
    |QPDiscriminator.IconClass| as IconClass,
    |QPDiscriminator.IconIntent| as IconIntent,
    |QPDiscriminator.PreferredContentId| as PreferredContentId,
	|QPDiscriminator.FrontModuleUrl| as FrontModuleUrl,
	|QPDiscriminator.ModuleName| as FrontModuleName
FROM |QPDiscriminator|
";

        public IEnumerable<ItemDefinitionPersistentData> GetAllItemDefinitions(int siteId, bool isStage, IDbTransaction transaction = null)
        {
            var query = _netNameQueryAnalyzer.PrepareQuery(CmdGetAll, siteId, isStage, potentiallyMissingColumns: _potentiallyMissingColumns);

            var cacheKey = query;
            var contentNetNames = _netNameQueryAnalyzer
                .GetContentNetNames(CmdGetAll, siteId, isStage)
                .ToArray();
            var cacheTags = _qpContentCacheTagNamingProvider
                .GetByContentNetNames(contentNetNames, siteId, isStage)
                .Select(n => n.Value)
                .ToArray();
            var expiry = _cacheSettings.ItemDefinitionCachePeriod;

            return _cacheProvider.GetOrAdd(
                cacheKey,
                cacheTags,
                expiry,
                () =>
                {
                    using var _ = _logger.BeginScopeWith(
                        ("unitOfWorkId", UnitOfWork.Id),
                        ("siteId", siteId),
                        ("isStage", isStage),
                        ("cacheKey", cacheKey),
                        ("cacheTags", cacheTags),
                        ("expiry", expiry));
                    _logger.LogTrace("Get all item definitions");
                    return UnitOfWork.Connection.Query<ItemDefinitionPersistentData>(query, transaction).ToList();
                });
        }
    }
}
