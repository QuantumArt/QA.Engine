using QA.DotNetCore.Engine.QpData.Persistent.Data;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.QpData.Persistent.Interfaces
{
    public interface IAbstractItemRepository
    {
        IEnumerable<AbstractItemPersistentData> GetPlainAllAbstractItems(int siteId, bool isStage);

        IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(int extensionId, IEnumerable<int> ids, bool isStage);
        
        IDictionary<int, AbstractItemM2mRelations> GetAbstractItemManyToManyData(IEnumerable<int> ids, bool isStage);
    }
}
