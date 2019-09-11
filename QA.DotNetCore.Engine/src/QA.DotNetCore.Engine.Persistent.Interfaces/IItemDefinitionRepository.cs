using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;
using System.Data;

namespace  QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IItemDefinitionRepository
    {
        IEnumerable<ItemDefinitionPersistentData> GetAllItemDefinitions(int siteId, bool isStage, IDbTransaction transaction = null);

        string ItemDefinitionNetName { get; }
    }
}
