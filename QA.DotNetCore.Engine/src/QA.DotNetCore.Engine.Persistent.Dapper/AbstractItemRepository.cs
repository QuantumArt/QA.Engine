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
            var cacheKey = $"{nameof(AbstractItemRepository)}.{nameof(GetPlainAllAbstractItems)}(" +
                $"{nameof(siteId)}:{siteId},{nameof(isStage)}:{isStage})";
            var cacheTags = _netNameQueryAnalyzer.GetContentNetNames(CmdGetAbstractItem, siteId, isStage)
                .Select(name => _qpContentCacheTagNamingProvider.Get(name, siteId, isStage))
                .ToArray();
            var expiry = _cacheSettings.SiteStructureCachePeriod;

            return _cacheProvider.GetOrAdd(cacheKey, cacheTags, expiry, GetActualPlainAbstractItems);

            IEnumerable<AbstractItemPersistentData> GetActualPlainAbstractItems()
            {
                var query = _netNameQueryAnalyzer.PrepareQuery(CmdGetAbstractItem, siteId, isStage);
                return UnitOfWork.Connection.Query<AbstractItemPersistentData>(query, transaction: transaction);
            }
        }

        /// <summary>
        /// Получить Content_item_id расширений
        /// </summary>
        /// <param name="extensionsContents">Словарь ID контента расширений и использующия их коллекция AbstractItems</param>
        /// <param name="isStage"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IEnumerable<int> GetAbstractItemExtensionIds(IDictionary<int, IEnumerable<int>> extensionsContents, bool isStage, IDbTransaction transaction = null)
        {
            const string selectDelimiter = " UNION ";
            var dbParams = new List<KeyValuePair<string, IEnumerable<int>>>();
            var extFieldsQuery = string.Join(selectDelimiter, BuildSelectQueries());

            using (var command = UnitOfWork.Connection.CreateCommand())
            {
                command.CommandText = extFieldsQuery;
                foreach (var param in dbParams)
                {
                    command.Parameters.Add(
                        SqlQuerySyntaxHelper.GetIdsDatatableParam(param.Key, param.Value, UnitOfWork.DatabaseType));
                }

                command.Transaction = transaction;

                var result = new List<int>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var extensionId = reader.GetInt32(0);
                        result.Add(extensionId);
                    }

                    return result;
                }
            }

            IEnumerable<string> BuildSelectQueries()
            {
                var withNoLock = SqlQuerySyntaxHelper.WithNoLock(UnitOfWork.DatabaseType);

                var queryPattern = $"SELECT CONTENT_ITEM_ID FROM {{0}} ext {withNoLock} JOIN {{1}} on Id = ext.itemid";
                const string idsDelimiter = ",";

                foreach (var item in extensionsContents)
                {
                    if (item.Key == 0)
                        continue;

                    var idsParam = GetAndSaveDbParam(item.Key, item.Value);
                    var extTableName = QpTableNameHelper.GetTableName(item.Key, isStage);
                    var idListTable = SqlQuerySyntaxHelper.IdList(UnitOfWork.DatabaseType, idsParam.Key, "ids");
                    yield return string.Format(queryPattern, extTableName, string.Join(idsDelimiter, idListTable));
                }
            }

            KeyValuePair<string, IEnumerable<int>> GetAndSaveDbParam(int extId, IEnumerable<int> aiIds)
            {
                var name = $"@ids_{extId}";
                var kvp = new KeyValuePair<string, IEnumerable<int>>(name, aiIds);
                dbParams.Add(kvp);
                return kvp;
            }
        }

        public IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(int extensionContentId,
            IEnumerable<int> ids,
            ContentPersistentData baseContent,
            bool loadAbstractItemFields, bool isStage, IDbTransaction transaction = null)
        {
            var extTableName = QpTableNameHelper.GetTableName(extensionContentId, isStage);
            var idListTable = SqlQuerySyntaxHelper.IdList(UnitOfWork.DatabaseType, "@ids", "ids");
            var withNoLock = SqlQuerySyntaxHelper.WithNoLock(UnitOfWork.DatabaseType);

            var extFieldsQuery =
                $@"SELECT * FROM {extTableName} ext {withNoLock}
                    INNER JOIN {idListTable} on Id = ext.itemid
                    {(loadAbstractItemFields ? $"INNER JOIN {baseContent.GetTableName(isStage)} ai on ai.Content_item_id = ext.itemid" : "")}";

            using (var command = UnitOfWork.Connection.CreateCommand())
            {
                command.CommandText = extFieldsQuery;
                command.Parameters.Add(SqlQuerySyntaxHelper.GetIdsDatatableParam("@Ids", ids, UnitOfWork.DatabaseType));
                command.Transaction = transaction;
                return LoadAbstractItemExtension(command);
            }
        }

        public IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionlessData(IEnumerable<int> ids,
            ContentPersistentData baseContent,
            bool isStage, IDbTransaction transaction = null)
        {
            var idListTable = SqlQuerySyntaxHelper.IdList(UnitOfWork.DatabaseType, "@ids", "ids");
            var withNoLock = SqlQuerySyntaxHelper.WithNoLock(UnitOfWork.DatabaseType);

            string extFieldsQuery =
                $@"SELECT * FROM {baseContent.GetTableName(isStage)} ai {withNoLock}
                    INNER JOIN {idListTable} on Id = ai.Content_item_id";

            using (var command = UnitOfWork.Connection.CreateCommand())
            {
                command.CommandText = extFieldsQuery;
                command.Parameters.Add(SqlQuerySyntaxHelper.GetIdsDatatableParam("@Ids", ids, UnitOfWork.DatabaseType));
                command.Transaction = transaction;
                return LoadAbstractItemExtension(command);
            }
        }

        private IDictionary<int, AbstractItemExtensionCollection> LoadAbstractItemExtension(IDbCommand command)
        {
            var result = new Dictionary<int, AbstractItemExtensionCollection>();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = 0;
                    var extensionCollection = new AbstractItemExtensionCollection();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var column = reader.GetName(i);
                        if (string.Equals(column, "Id", StringComparison.OrdinalIgnoreCase))
                            id = Decimal.ToInt32(reader.GetDecimal(i));
                        else
                        {
                            var val = reader.GetValue(i);
                            extensionCollection.Add(column, val is DBNull ? null : val);
                        }
                    }

                    if (id > 0) result[id] = extensionCollection;
                }

                return result;
            }
        }

        public IDictionary<int, M2mRelations> GetManyToManyData(IEnumerable<int> ids, bool isStage, IDbTransaction transaction = null)
        {
            var m2MTableName = QpTableNameHelper.GetM2MTableName(isStage);
            var idListTable = SqlQuerySyntaxHelper.IdList(UnitOfWork.DatabaseType, "@ids", "ids");
            var withNoLock = SqlQuerySyntaxHelper.WithNoLock(UnitOfWork.DatabaseType);

            var query = $@"
                SELECT link_id, item_id, linked_item_id
                FROM {m2MTableName} link {withNoLock}
                INNER JOIN {idListTable} on Id = link.item_id";

            using (var command = UnitOfWork.Connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(SqlQuerySyntaxHelper.GetIdsDatatableParam("@Ids", ids, UnitOfWork.DatabaseType));
                command.Transaction = transaction;
                var result = new Dictionary<int, M2mRelations>();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var itemId = Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("item_id")));
                        if (!result.ContainsKey(itemId)) result[itemId] = new M2mRelations();

                        result[itemId]
                            .AddRelation(Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("link_id"))),
                                Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("linked_item_id"))));
                    }

                    return result;
                }
            }
        }
    }
}
