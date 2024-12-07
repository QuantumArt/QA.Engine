using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces.Logging;
using QA.DotNetCore.Engine.QpData.Settings;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class AbstractItemRepository : IAbstractItemRepository
    {
        private static readonly IReadOnlyDictionary<int, M2MRelations> _emptyResult =
            new Dictionary<int, M2MRelations>();

        private readonly ILogger _logger;
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        private readonly ICacheProvider _cacheProvider;
        private readonly QpSiteStructureCacheSettings _cacheSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly INetNameQueryAnalyzer _netNameQueryAnalyzer;
        private IUnitOfWork _unitOfWork;

        public AbstractItemRepository(
            IServiceProvider serviceProvider,
            INetNameQueryAnalyzer netNameQueryAnalyzer,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            ICacheProvider cacheProvider,
            QpSiteStructureCacheSettings cacheSettings,
            ILogger<AbstractItemRepository> logger)
        {
            _serviceProvider = serviceProvider;
            _netNameQueryAnalyzer = netNameQueryAnalyzer;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheProvider = cacheProvider;
            _cacheSettings = cacheSettings;
            _logger = logger;
        }

        protected IUnitOfWork UnitOfWork
        {
            get
            {
                if (_unitOfWork == null)
                {
                    _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
                    using var _ = _logger.BeginScopeWith(("unitOfWorkId", _unitOfWork.Id));
                    _logger.LogTrace("Received UnitOfWork from ServiceProvider");
                }
                return _unitOfWork;
            }
        }

        //запрос с использованием NetName таблиц и столбцов
        private const string CmdGetAbstractItem = @"
SELECT
    ai.content_item_id AS Id,
    ai.|QPAbstractItem.Name| as Alias,
    ai.|QPAbstractItem.Title| as Title,
    ai.|QPAbstractItem.Parent| AS ParentId,
    ai.|QPAbstractItem.IsVisible| AS IsVisible,
    ai.|QPAbstractItem.ZoneName| AS ZoneName,
    ai.|QPAbstractItem.IndexOrder| AS IndexOrder,
    ai.|QPAbstractItem.ExtensionId| AS ExtensionId,
    ai.|QPAbstractItem.VersionOf| AS VersionOfId,
    def.|QPDiscriminator.Name| as Discriminator,
    def.|QPDiscriminator.IsPage| as IsPage,
    CASE WHEN ai.STATUS_TYPE_ID IN (SELECT st.STATUS_TYPE_ID FROM STATUS_TYPE st WHERE st.STATUS_TYPE_NAME=N'Published') THEN 1 ELSE 0 END AS Published
FROM |QPAbstractItem| ai
INNER JOIN |QPDiscriminator| def on ai.|QPAbstractItem.Discriminator| = def.content_item_id
";

        public IEnumerable<AbstractItemPersistentData> GetPlainAllAbstractItems(int siteId, bool isStage,
            IDbTransaction transaction = null)
        {
            var query = _netNameQueryAnalyzer.PrepareQuery(CmdGetAbstractItem, siteId, isStage);
            var cacheKey = $"{nameof(AbstractItemRepository)}.{nameof(GetPlainAllAbstractItems)}({siteId},{isStage})";
            var contentNetNames = _netNameQueryAnalyzer
                .GetContentNetNames(CmdGetAbstractItem, siteId, isStage)
                .ToArray();
            var cacheTags = _qpContentCacheTagNamingProvider
                    .GetByContentNetNames(contentNetNames, siteId, isStage)
                    .Select(n => n.Value)
                    .ToArray();
            var expiry = _cacheSettings.SiteStructureCachePeriod;

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
                    _logger.LogTrace("Get all abstract items");
                    return UnitOfWork.Connection.Query<AbstractItemPersistentData>(query, transaction: transaction);
                });
        }

        /// <summary>
        /// Получить Content_item_id расширений
        /// </summary>
        /// <param name="extensionContentIds">Словарь ID контента расширений и использующия их коллекция AbstractItems</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IEnumerable<int> GetAbstractItemExtensionIds(IReadOnlyCollection<int> extensionContentIds,
            IDbTransaction transaction = null)
        {
            const string idsTableParameterName = "@ids";

            string withNoLock = SqlQuerySyntaxHelper.WithNoLock(UnitOfWork.DatabaseType);
            string idListTableName = SqlQuerySyntaxHelper.IdList(UnitOfWork.DatabaseType, idsTableParameterName, "ids");
            string extensionItemsQuery = @$"
SELECT CONTENT_ITEM_ID
FROM CONTENT_ITEM ext {withNoLock}
JOIN {idListTableName} on Id = ext.CONTENT_ID";

            IDataParameter parameter = SqlQuerySyntaxHelper.GetIdsDatatableParam(
                idsTableParameterName,
                extensionContentIds.Where(id => id != 0),
                UnitOfWork.DatabaseType);

            using var _ = _logger.BeginScopeWith(
                ("unitOfWorkId", UnitOfWork.Id),
                ("extensionContentIds", extensionContentIds));
            _logger.LogTrace("Get abstract items extension ids");

            using var command = UnitOfWork.Connection.CreateCommand();
            command.CommandText = extensionItemsQuery;
            command.Transaction = transaction;
            command.Parameters.Add(parameter);

            using var reader = command.ExecuteReader();

            var extensionIds = new List<int>(extensionContentIds.Count);

            while (reader.Read())
            {
                var extensionId = Convert.ToInt32(reader.GetDecimal(0));
                extensionIds.Add(extensionId);
            }

            return extensionIds;
        }

        public IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(
            int extensionContentId,
            ContentPersistentData baseContent,
            bool loadAbstractItemFields,
            bool isStage,
            IDbTransaction transaction = null)
        {
            var extTableName = QpTableNameHelper.GetTableName(extensionContentId, isStage);
            var withNoLock = SqlQuerySyntaxHelper.WithNoLock(UnitOfWork.DatabaseType);
            var extFields = loadAbstractItemFields ? ", ext.*" : "";
            var query = $@"
SELECT cast(ai.content_item_id as numeric) as Id{extFields}, ai.*
FROM {extTableName} ext {withNoLock}
JOIN {baseContent.GetTableName(isStage)} ai {withNoLock} on ai.content_item_id = ext.itemid";

            using var _ = _logger.BeginScopeWith(
                ("unitOfWorkId", UnitOfWork.Id),
                ("extensionContentId", extensionContentId),
                ("loadAbstractItemFields", loadAbstractItemFields),
                ("isStage", isStage));
            _logger.LogTrace("Get abstract items extension data");

            using var command = UnitOfWork.Connection.CreateCommand();
            command.CommandText = query;
            command.Transaction = transaction;
            return LoadAbstractItemExtension(command);
        }

        public IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionlessData(
            int[] ids,
            ContentPersistentData baseContent,
            bool isStage,
            IDbTransaction transaction = null)
        {
            var idListTable = SqlQuerySyntaxHelper.IdList(UnitOfWork.DatabaseType, "@ids", "ids");
            var withNoLock = SqlQuerySyntaxHelper.WithNoLock(UnitOfWork.DatabaseType);

            string extFieldsQuery = $@"
SELECT * FROM {baseContent.GetTableName(isStage)} ai {withNoLock}
JOIN {idListTable} on Id = ai.Content_item_id";

            using var _ = _logger.BeginScopeWith(
                ("unitOfWorkId", UnitOfWork.Id),
                ("ids", ids),
                ("isStage", isStage));
            _logger.LogTrace("Get abstract items extensionless data");

            using var command = UnitOfWork.Connection.CreateCommand();
            command.CommandText = extFieldsQuery;
            command.Parameters.Add(SqlQuerySyntaxHelper.GetIdsDatatableParam("@Ids", ids, UnitOfWork.DatabaseType));
            command.Transaction = transaction;

            return LoadAbstractItemExtension(command);
        }

        private Dictionary<int, AbstractItemExtensionCollection> LoadAbstractItemExtension(IDbCommand command)
        {
            var result = new Dictionary<int, AbstractItemExtensionCollection>();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = 0;
                var extensionCollection = new AbstractItemExtensionCollection();
                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var column = reader.GetName(i);
                    if (string.Equals(column, "Id", StringComparison.OrdinalIgnoreCase))
                    {
                        id = decimal.ToInt32(reader.GetDecimal(i));
                    }
                    else
                    {
                        var val = reader.GetValue(i);
                        extensionCollection.Add(column, val is DBNull ? null : val);
                    }
                }

                if (id > 0)
                {
                    result[id] = extensionCollection;
                }
            }

            return result;
        }

        public IDictionary<int, M2MRelations> GetManyToManyData(int[] itemIds, bool isStage, IDbTransaction transaction = null)
        {
            var m2MTableName = QpTableNameHelper.GetM2MTableName(isStage);
            var idListTable = SqlQuerySyntaxHelper.IdList(UnitOfWork.DatabaseType, "@ids", "ids");
            var withNoLock = SqlQuerySyntaxHelper.WithNoLock(UnitOfWork.DatabaseType);

            var query = $@"
SELECT link_id, item_id, linked_item_id
FROM {m2MTableName} link {withNoLock}
JOIN {idListTable} on Id = link.item_id
JOIN content_item ci {withNoLock} on ci.content_item_id = link.linked_item_id
WHERE ci.archive = 0";

            using var _ = _logger.BeginScopeWith(
                ("unitOfWorkId", UnitOfWork.Id),
                ("ids", itemIds),
                ("isStage", isStage));
            _logger.LogTrace("Get M2M data for articles");

            using var command = UnitOfWork.Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(SqlQuerySyntaxHelper.GetIdsDatatableParam("@Ids", itemIds, UnitOfWork.DatabaseType));
            command.Transaction = transaction;
            using var reader = command.ExecuteReader();
            return ReadM2MRelations(reader);
        }

        public IDictionary<int, M2MRelations> GetManyToManyDataByContents(
            int[] contentIds,
            bool isStage,
            IDbTransaction transaction = null)
        {
            const string idsTableParameterName = "@ids";

            string withNoLock = SqlQuerySyntaxHelper.WithNoLock(UnitOfWork.DatabaseType);
            string idListTableName = SqlQuerySyntaxHelper.IdList(UnitOfWork.DatabaseType, idsTableParameterName, "ids");
            string m2MTableName = QpTableNameHelper.GetM2MTableName(isStage);

            var query = $@"
SELECT e.content_id, link_id, item_id, linked_item_id
FROM {m2MTableName} link {withNoLock}
JOIN CONTENT_ITEM e {withNoLock} ON e.CONTENT_ITEM_ID = link.item_id
JOIN {idListTableName} on Id = e.CONTENT_ID";

            using var _ = _logger.BeginScopeWith(
                ("unitOfWorkId", UnitOfWork.Id),
                ("contentIds", contentIds),
                ("isStage", isStage));
            _logger.LogTrace("Get M2M data for contents");

            using var command = UnitOfWork.Connection.CreateCommand();
            command.CommandText = query;
            command.Transaction = transaction;
            command.Parameters.Add(
                SqlQuerySyntaxHelper.GetIdsDatatableParam("@Ids", contentIds, UnitOfWork.DatabaseType));
            using var reader = command.ExecuteReader();
            return ReadM2MRelations(reader);
        }

        private static Dictionary<int, M2MRelations> ReadM2MRelations(IDataReader reader)
        {
            var result = new Dictionary<int, M2MRelations>();
            while (reader.Read())
            {
                var itemId = Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("item_id")));
                if (!result.ContainsKey(itemId))
                {
                    result[itemId] = new M2MRelations();
                }

                result[itemId].AddRelation(
                    Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("link_id"))),
                    Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("linked_item_id"))));
            }

            return result;
        }
    }
}
