using Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class ItemDefinitionRepository : IItemDefinitionRepository
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly INetNameQueryAnalyzer _netNameQueryAnalyzer;

        public ItemDefinitionRepository(IServiceProvider serviceProvider, INetNameQueryAnalyzer netNameQueryAnalyzer)
        {
            _serviceProvider = serviceProvider;
            _netNameQueryAnalyzer = netNameQueryAnalyzer;
        }

        protected IUnitOfWork UnitOfWork { get { return _serviceProvider.GetRequiredService<IUnitOfWork>(); } }

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
            return UnitOfWork.Connection.Query<ItemDefinitionPersistentData>(query, transaction).ToList();
        }
    }
}
