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
using NLog;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class ItemDefinitionRepository : IItemDefinitionRepository
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
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
            QpSiteStructureCacheSettings cacheSettings)
        {
            _serviceProvider = serviceProvider;
            _netNameQueryAnalyzer = netNameQueryAnalyzer;
            _cacheSettings = cacheSettings;
            _cacheProvider = cacheProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
        }

        protected IUnitOfWork UnitOfWork
        {
            get
            {
                var uow = _serviceProvider.GetRequiredService<IUnitOfWork>();
                _logger.ForTraceEvent()
                    .Message("Received UnitOfWork {unitOfWorkId} from ServiceProvider", uow.Id)
                    .Log();
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
            var connection = UnitOfWork.Connection;
            var query = _netNameQueryAnalyzer.PrepareQuery(CmdGetAll, siteId, isStage, potentiallyMissingColumns: _potentiallyMissingColumns);

            var cacheKey = query;
            var cacheTags = _netNameQueryAnalyzer.GetContentNetNames(CmdGetAll, siteId, isStage)
                .Select(name => _qpContentCacheTagNamingProvider.GetByNetName(name, siteId, isStage))
                .ToArray();
            var expiry = _cacheSettings.ItemDefinitionCachePeriod;

            return _cacheProvider.GetOrAdd(
                cacheKey,
                cacheTags,
                expiry,
                () => connection.Query<ItemDefinitionPersistentData>(query, transaction).ToList());
        }
    }
}
