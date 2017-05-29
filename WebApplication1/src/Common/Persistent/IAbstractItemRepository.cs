using Common.PageModel;
using Common.Persistent.Data;
using System.Collections.Generic;

namespace Common.Persistent
{
    public interface IAbstractItemRepository
    {
        IEnumerable<AbstractItemPersistentData> GetPlainAllAbstractItems();

        IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(int extensionId, int[] ids);
    }
}
