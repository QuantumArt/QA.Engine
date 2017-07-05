using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QA.DotNetCore.Engine.QpData.Persistent.Data;
using System.Data;
using Dapper;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class ItemDefinitionRepository : IItemDefinitionRepository
    {
        private readonly IDbConnection _connection;

        public ItemDefinitionRepository(IUnitOfWork uow)
        {
            _connection = uow.Connection;
        }

        private const string CmdGetAll = @"
SELECT
    CONTENT_ITEM_ID as Id,
    Name as Discriminator,
    FriendlyDescription as TypeName
FROM {0}
";
        private const string CmdGetItemDefinitionContentId = @"
SELECT CONTENT_ID AS ContentId
FROM content
WHERE NET_CONTENT_NAME = 'QPDiscriminator' and SITE_ID = {0}";

        string GetItemDefinitionTable(int siteId, bool isStage)
        {
            var result = _connection.Query(string.Format(CmdGetItemDefinitionContentId, siteId)).First();
            var stageOrLiveToken = isStage ? "stage" : "live";
            return $"content_{result.ContentId}_{stageOrLiveToken}_new";
        }

        public IEnumerable<ItemDefinitionPersistentData> GetAllItemDefinitions(int siteId, bool isStage)
        {
            return _connection.Query<ItemDefinitionPersistentData>(string.Format(CmdGetAll, GetItemDefinitionTable(siteId, isStage))).ToList();
        }
    }
}
