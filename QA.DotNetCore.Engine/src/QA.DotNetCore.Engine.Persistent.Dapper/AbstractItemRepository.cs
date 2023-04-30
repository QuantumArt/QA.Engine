using Dapper;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class AbstractItemRepository : IAbstractItemRepository
    {
        private static readonly IReadOnlyDictionary<int, M2mRelations> _emptyResult = new Dictionary<int, M2mRelations>();

        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        private readonly ICacheProvider _cacheProvider;
        private readonly QpSiteStructureCacheSettings _cacheSettings;
        private readonly IServiceProvider _serviceProvider;
        private readonly INetNameQueryAnalyzer _netNameQueryAnalyzer;

        public AbstractItemRepository(
            IServiceProvider serviceProvider,
            INetNameQueryAnalyzer netNameQueryAnalyzer,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            ICacheProvider cacheProvider,
            QpSiteStructureCacheSettings cacheSettings)
        {
            _serviceProvider = serviceProvider;
            _netNameQueryAnalyzer = netNameQueryAnalyzer;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheProvider = cacheProvider;
            _cacheSettings = cacheSettings;
        }

        protected IUnitOfWork UnitOfWork { get { return _serviceProvider.GetRequiredService<IUnitOfWork>(); } }

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

        public IEnumerable<AbstractItemPersistentData> GetPlainAllAbstractItems(int siteId, bool isStage, IDbTransaction transaction = null)
        {
            var connection = UnitOfWork.Connection;
            var query = _netNameQueryAnalyzer.PrepareQuery(CmdGetAbstractItem, siteId, isStage);

            var cacheKey = $"{nameof(AbstractItemRepository)}.{nameof(GetPlainAllAbstractItems)}({siteId},{isStage})";
            var cacheTags = _netNameQueryAnalyzer.GetContentNetNames(CmdGetAbstractItem, siteId, isStage)
                .Select(name => _qpContentCacheTagNamingProvider.GetByNetName(name, siteId, isStage))
                .ToArray();
            var expiry = _cacheSettings.SiteStructureCachePeriod;

            return _cacheProvider.GetOrAdd(
                cacheKey,
                cacheTags,
                expiry,
                () => connection.Query<AbstractItemPersistentData>(query, transaction: transaction));
        }

        /// <summary>
        /// Получить Content_item_id расширений
        /// </summary>
        /// <param name="extensionContentIds">Словарь ID контента расширений и использующия их коллекция AbstractItems</param>
        /// <param name="isStage"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IEnumerable<int> GetAbstractItemExtensionIds(IReadOnlyCollection<int> extensionContentIds, IDbTransaction transaction = null)
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

            using var command = UnitOfWork.Connection.CreateCommand();
            command.CommandText = extensionItemsQuery;
            command.Transaction = transaction;
            _ = command.Parameters.Add(parameter);

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
SELECT cast(ai.content_item_id as numeric) as Id, ai.*{extFields} 
FROM {extTableName} ext {withNoLock}
JOIN {baseContent.GetTableName(isStage)} ai {withNoLock} on ai.content_item_id = ext.itemid";

            using var command = UnitOfWork.Connection.CreateCommand();
            command.CommandText = query;
            command.Transaction = transaction;

            return LoadAbstractItemExtension(command);
        }

        public IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionlessData(
            IEnumerable<int> ids,
            ContentPersistentData baseContent,
            bool isStage,
            IDbTransaction transaction = null)
        {
            var idListTable = SqlQuerySyntaxHelper.IdList(UnitOfWork.DatabaseType, "@ids", "ids");
            var withNoLock = SqlQuerySyntaxHelper.WithNoLock(UnitOfWork.DatabaseType);

            string extFieldsQuery = $@"
SELECT * FROM {baseContent.GetTableName(isStage)} ai {withNoLock}
JOIN {idListTable} on Id = ai.Content_item_id";

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

        public IDictionary<int, M2mRelations> GetManyToManyData(IEnumerable<int> itemIds, bool isStage, IDbTransaction transaction = null)
        {
            var m2MTableName = QpTableNameHelper.GetM2MTableName(isStage);
            var idListTable = SqlQuerySyntaxHelper.IdList(UnitOfWork.DatabaseType, "@ids", "ids");
            var withNoLock = SqlQuerySyntaxHelper.WithNoLock(UnitOfWork.DatabaseType);

            var query = $@"
SELECT link_id, item_id, linked_item_id
FROM {m2MTableName} link {withNoLock}
JOIN {idListTable} on Id = link.item_id";

            using var command = UnitOfWork.Connection.CreateCommand();
            command.CommandText = query;
            command.Parameters.Add(SqlQuerySyntaxHelper.GetIdsDatatableParam("@Ids", itemIds, UnitOfWork.DatabaseType));
            command.Transaction = transaction;
            var result = new Dictionary<int, M2mRelations>();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var itemId = Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("item_id")));
                if (!result.ContainsKey(itemId))
                {
                    result[itemId] = new M2mRelations();
                }

                result[itemId].AddRelation(
                    Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("link_id"))),
                    Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("linked_item_id"))));
            }

            return result;
        }

        public IEnumerable<IReadOnlyDictionary<int, M2mRelations>> GetManyToManyDataByContent(
            IReadOnlyCollection<int> contentIds,
            bool isStage,
            IDbTransaction transaction = null)
        {
            const string idsTableParameterName = "@ids";

            string withNoLock = SqlQuerySyntaxHelper.WithNoLock(UnitOfWork.DatabaseType);
            string idListTableName = SqlQuerySyntaxHelper.IdList(UnitOfWork.DatabaseType, idsTableParameterName, "ids");
            string m2MTableName = QpTableNameHelper.GetM2MTableName(isStage);

            IDataParameter parameter = SqlQuerySyntaxHelper.GetIdsDatatableParam(
                idsTableParameterName,
                contentIds.Where(id => id != 0),
                UnitOfWork.DatabaseType);
            var query = $@"
SELECT e.content_id, link_id, item_id, linked_item_id
FROM {m2MTableName} link {withNoLock}
JOIN CONTENT_ITEM e {withNoLock} ON e.CONTENT_ITEM_ID = link.item_id
JOIN {idListTableName} on Id = e.CONTENT_ID";

            using var command = UnitOfWork.Connection.CreateCommand();
            command.CommandText = query;
            command.Transaction = transaction;
            _ = command.Parameters.Add(parameter);

            var groupedRelations = new Dictionary<int, Dictionary<int, M2mRelations>>();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int extensionId = Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("content_id")));
                    int itemId = Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("item_id")));

                    if (!groupedRelations.TryGetValue(extensionId, out var itemInfo))
                    {
                        itemInfo = new Dictionary<int, M2mRelations>();
                        groupedRelations.Add(extensionId, itemInfo);
                    }

                    if (!itemInfo.TryGetValue(itemId, out var relations))
                    {
                        relations = new M2mRelations();
                        itemInfo.Add(itemId, relations);
                    }

                    relations.AddRelation(
                        Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("link_id"))),
                        Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("linked_item_id"))));
                }
            }

            // Return in the same order as input collection.
            foreach (int extensionId in contentIds)
            {
                yield return groupedRelations.TryGetValue(extensionId, out var itemInfos) ? itemInfos : _emptyResult;
            }
        }
    }
}
