using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Persistent.Data;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class AbstractItemRepository : IAbstractItemRepository
    {
        private readonly IDbConnection _connection;
        private readonly INetNameQueryAnalyzer _netNameQueryAnalyzer;

        public AbstractItemRepository(IUnitOfWork uow, INetNameQueryAnalyzer netNameQueryAnalyzer)
        {
            _connection = uow.Connection;
            _netNameQueryAnalyzer = netNameQueryAnalyzer;
        }

        //запрос с использованием NetName таблиц и столбцов
        private const string CmdGetAbstractItem = @"
SELECT
    ai.content_item_id AS Id,
    ai.[|QPAbstractItem.Name|] as Alias,
    ai.[|QPAbstractItem.Title|] as Title,
    ai.Visible,
    ai.[|QPAbstractItem.Parent|] AS ParentId,
    ai.[|QPAbstractItem.IsVisible|] AS IsVisible,
    ai.[|QPAbstractItem.ZoneName|] AS ZoneName,
    ai.[|QPAbstractItem.IndexOrder|] AS IndexOrder,
    ai.[|QPAbstractItem.ExtensionId|] AS ExtensionId,
    def.[|QPDiscriminator.Name|] as Discriminator,
    def.[|QPDiscriminator.IsPage|] as IsPage
FROM [|QPAbstractItem|] ai
INNER JOIN [|QPDiscriminator|] def on ai.[|QPAbstractItem.Discriminator|] = def.content_item_id
";

        private const string CmdGetExtension = @"[qa_extend_items]";
        private const string CmdGetManyToMany = @"[qa_extend_items_m2m]";

        public IEnumerable<AbstractItemPersistentData> GetPlainAllAbstractItems(int siteId, bool isStage)
        {
            var query = _netNameQueryAnalyzer.PrepareQuery(CmdGetAbstractItem, siteId, isStage);
            return _connection.Query<AbstractItemPersistentData>(query);
        }

        public IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(int extensionId, IEnumerable<int> ids, bool isStage)
        {
            if (!(_connection is SqlConnection))
                throw new NotImplementedException("GetAbstractItemExtensionData can be executed in MS SQL only");

            var idsParameter = new List<SqlDataRecord>();
            var metaData = new SqlMetaData[] { new SqlMetaData("Id", SqlDbType.Int) };
            foreach (var id in ids)
            {
                var record = new SqlDataRecord(metaData);
                record.SetInt32(0, id);
                idsParameter.Add(record);
            }

            using (var command = (_connection as SqlConnection).CreateCommand())
            {
                command.CommandText = CmdGetExtension;
                command.CommandType = CommandType.StoredProcedure;

                var tvpParam = command.Parameters.AddWithValue("@Ids", idsParameter);
                var isLive = command.Parameters.AddWithValue("@isLive", !isStage);
                var contentId = command.Parameters.AddWithValue("@contentId", extensionId);

                isLive.SqlDbType = SqlDbType.Bit;
                contentId.SqlDbType = SqlDbType.Int;
                tvpParam.SqlDbType = SqlDbType.Structured;

                var result = new Dictionary<int, AbstractItemExtensionCollection>();
                
                using (var reader = command.ExecuteReader())
                {
                    //DataTable not available in .NetStandart v1.6, go with reader
                    while (reader.Read())
                    {
                        int id = 0;
                        var extensionCollection = new AbstractItemExtensionCollection();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var column = reader.GetName(i);
                            if (column == "Id")
                                id = reader.GetInt32(i);
                            else
                            {
                                var val = reader.GetValue(i);
                                extensionCollection.Add(column, val is DBNull ? null : val);
                            }
                                
                        }

                        if (id > 0)
                            result[id] = extensionCollection;
                    }

                    return result;
                }
            }

        }

        public IDictionary<int, AbstractItemM2mRelations> GetAbstractItemManyToManyData(IEnumerable<int> ids, bool isStage)
        {
            if (!(_connection is SqlConnection))
                throw new NotImplementedException("GetAbstractItemManyToManyData can be executed in MS SQL only");

            var idsParameter = new List<SqlDataRecord>();
            var metaData = new SqlMetaData[] { new SqlMetaData("Id", SqlDbType.Int) };
            foreach (var id in ids)
            {
                var record = new SqlDataRecord(metaData);
                record.SetInt32(0, id);
                idsParameter.Add(record);
            }

            using (var command = (_connection as SqlConnection).CreateCommand())
            {
                command.CommandText = CmdGetManyToMany;
                command.CommandType = CommandType.StoredProcedure;

                var tvpParam = command.Parameters.AddWithValue("@Ids", idsParameter);
                var isLive = command.Parameters.AddWithValue("@isLive", !isStage);

                isLive.SqlDbType = SqlDbType.Bit;
                tvpParam.SqlDbType = SqlDbType.Structured;

                var result = new Dictionary<int, AbstractItemM2mRelations>();
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var itemId = Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("item_id")));
                        if (!result.ContainsKey(itemId))
                            result[itemId] = new AbstractItemM2mRelations();

                        result[itemId].AddRelation(
                            Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("link_id"))),
                            Convert.ToInt32(reader.GetDecimal(reader.GetOrdinal("linked_item_id"))));
                    }

                    return result;
                }
            }
        }
    }
}
