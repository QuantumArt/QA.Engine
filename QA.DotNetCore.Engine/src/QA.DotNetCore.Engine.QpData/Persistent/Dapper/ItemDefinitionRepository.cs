using Dapper;
using QA.DotNetCore.Engine.QpData.Persistent.Data;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class ItemDefinitionRepository : IItemDefinitionRepository
    {
        private readonly IDbConnection _connection;
        private readonly INetNameQueryAnalyzer _netNameQueryAnalyzer;

        public ItemDefinitionRepository(IUnitOfWork uow, INetNameQueryAnalyzer netNameQueryAnalyzer)
        {
            _connection = uow.Connection;
            _netNameQueryAnalyzer = netNameQueryAnalyzer;
        }

        private const string CmdGetAll = @"
SELECT
    CONTENT_ITEM_ID as Id,
    [|QPDiscriminator.Name|] as Discriminator,
    [|QPDiscriminator.TypeName|] as TypeName
FROM [|QPDiscriminator|]
";

        public IEnumerable<ItemDefinitionPersistentData> GetAllItemDefinitions(int siteId, bool isStage)
        {
            var query = _netNameQueryAnalyzer.PrepareQuery(CmdGetAll, siteId, isStage);
            return _connection.Query<ItemDefinitionPersistentData>(query).ToList();
        }
    }
}
