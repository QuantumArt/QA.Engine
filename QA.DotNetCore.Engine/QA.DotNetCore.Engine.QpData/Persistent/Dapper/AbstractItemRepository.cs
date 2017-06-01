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
    internal class AbstractItemRepository : IAbstractItemRepository
    {
        private readonly IDbConnection _connection;

        public AbstractItemRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        private const string CmdGetAbstractItemContentId = @"
SELECT CONTENT_ID AS ContentId
FROM content
WHERE NET_CONTENT_NAME = 'QPAbstractItem'";

        private const string CmdGetItemDefinitionContentId = @"
SELECT CONTENT_ID AS ContentId
FROM content
WHERE NET_CONTENT_NAME = 'QPDiscriminator'";

        private const string CmdGetAbstractItem = @"
SELECT
    ai.content_item_id AS Id,
    ai.Name as Alias,
    ai.Title,
    ai.Visible,
    ai.Parent AS ParentId,
    ai.ZoneName,
    ai.ExtensionId,
    def.Name as Discriminator,
    def.IsPage
FROM {0} ai
INNER JOIN {1} def on ai.Discriminator = def.content_item_id
";

        private const string CmdGetExtension = @"[qa_extend_items]";

        string GetAbstractItemTable()
        {
            var result =  _connection.Query(CmdGetAbstractItemContentId).First();
            return $"content_{result.ContentId}_stage_new";
        }

        string GetItemDefinitionTable()
        {
            var result = _connection.Query(CmdGetItemDefinitionContentId).First();
            return $"content_{result.ContentId}_stage_new";
        }

        public IEnumerable<AbstractItemPersistentData> GetPlainAllAbstractItems()
        {
            return _connection.Query<AbstractItemPersistentData>(string.Format(CmdGetAbstractItem, GetAbstractItemTable(), GetItemDefinitionTable()));
        }

        public IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(int extensionId, int[] ids)
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
                var isLive = command.Parameters.AddWithValue("@isLive", false);
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
                                extensionCollection.Add(column, reader.GetValue(i));
                        }

                        if (id > 0)
                            result[id] = extensionCollection;
                    }

                    return result;
                }
            }

        }
    }
}
