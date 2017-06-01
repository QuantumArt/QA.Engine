using QA.DotNetCore.Engine.QpData.Persistent.Data;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.QpData.Persistent.Interfaces
{
    public interface IAbstractItemRepository
    {
        IEnumerable<AbstractItemPersistentData> GetPlainAllAbstractItems();

        IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(int extensionId, int[] ids);
    }
}
