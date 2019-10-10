using Dapper;
using System.Collections.Generic;
using System.Data;
using System;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class AbstractItemRepository : IAbstractItemRepository
    {
        private readonly IUnitOfWork _uow;
        private readonly INetNameQueryAnalyzer _netNameQueryAnalyzer;

        public AbstractItemRepository(IUnitOfWork uow, INetNameQueryAnalyzer netNameQueryAnalyzer)
        {
            _uow = uow;
            _netNameQueryAnalyzer = netNameQueryAnalyzer;
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

        private const string CmdGetExtensionFields = @"SELECT * FROM {0} ext {1} INNER JOIN {2} on Id = ext.itemid {3}";

        private const string CmdManyToMany = @"SELECT link_id, item_id, linked_item_id
          FROM {0} link {1} INNER JOIN {2} on Id = link.item_id";

        public string AbstractItemNetName => "QPAbstractItem";

        public IEnumerable<AbstractItemPersistentData> GetPlainAllAbstractItems(int siteId, bool isStage, IDbTransaction transaction = null)
        {
            var query = _netNameQueryAnalyzer.PrepareQuery(CmdGetAbstractItem, siteId, isStage);
            return _uow.Connection.Query<AbstractItemPersistentData>(query, transaction);
        }

        public IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(int extensionContentId,
            IEnumerable<int> ids,
            ContentPersistentData baseContent,
            bool loadAbstractItemFields, bool isStage, IDbTransaction transaction = null)
        {
            var extTableName = QpTableNameHelper.GetTableName(extensionContentId, isStage);
            var idList = SqlQuerySyntaxHelper.IdList(_uow.DatabaseType, "@ids", "ids");

            string extFieldsQuery = string.Format(CmdGetExtensionFields,
                extTableName,
                SqlQuerySyntaxHelper.WithNoLock(_uow.DatabaseType),
                idList,
                loadAbstractItemFields ? $"INNER JOIN {baseContent.GetTableName(isStage)} ai on ai.Content_item_id = ext.itemid" : "");

            using (var command = _uow.Connection.CreateCommand())
            {
                command.CommandText = extFieldsQuery;
                command.Parameters.Add(SqlQuerySyntaxHelper.GetIdsDatatableParam("@Ids", ids, _uow.DatabaseType));
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
            string m2mTableName = QpTableNameHelper.GetM2MTableName(isStage);
            var idList = SqlQuerySyntaxHelper.IdList(_uow.DatabaseType, "@ids", "ids");

            string query = string.Format(CmdManyToMany,
                m2mTableName,
                SqlQuerySyntaxHelper.WithNoLock(_uow.DatabaseType),
                idList);

            using (var command = _uow.Connection.CreateCommand())
            {
                command.CommandText = query;
                command.Parameters.Add(SqlQuerySyntaxHelper.GetIdsDatatableParam("@Ids", ids, _uow.DatabaseType));
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
