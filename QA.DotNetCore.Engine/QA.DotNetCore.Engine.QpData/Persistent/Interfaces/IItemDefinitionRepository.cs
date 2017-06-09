using QA.DotNetCore.Engine.QpData.Persistent.Data;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.QpData.Persistent.Interfaces
{
    public interface IItemDefinitionRepository
    {
        IEnumerable<ItemDefinitionPersistentData> GetAllItemDefinitions(int siteId, bool isStage);
    }
}
