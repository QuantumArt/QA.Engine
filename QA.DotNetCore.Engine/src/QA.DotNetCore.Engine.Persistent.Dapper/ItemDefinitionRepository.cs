using Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces;
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
    |QPDiscriminator.Name| as Discriminator,
    |QPDiscriminator.TypeName| as TypeName,
    |QPDiscriminator.IsPage| as IsPage,
    |QPDiscriminator.Title| as Title,
    |QPDiscriminator.Description| as Description,
    |QPDiscriminator.IconUrl| as IconUrl,
    |QPDiscriminator.IconClass| as IconClass,
    |QPDiscriminator.IconIntent| as IconIntent,
    |QPDiscriminator.PreferredContentId| as PreferredContentId
FROM |QPDiscriminator|
";

        public IEnumerable<ItemDefinitionPersistentData> GetAllItemDefinitions(int siteId, bool isStage, IDbTransaction transaction = null)
        {
            var query = _netNameQueryAnalyzer.PrepareQuery(CmdGetAll, siteId, isStage);
            return _connection.Query<ItemDefinitionPersistentData>(query, transaction).ToList();
        }
    }
}
