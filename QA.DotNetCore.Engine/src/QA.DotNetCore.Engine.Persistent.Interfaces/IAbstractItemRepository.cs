using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;

namespace  QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IAbstractItemRepository
    {
        IEnumerable<AbstractItemPersistentData> GetPlainAllAbstractItems(int siteId, bool isStage);

        IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(int extensionContentId, IEnumerable<int> ids, bool loadAbstractItemFields, bool isStage);

        IDictionary<int, M2mRelations> GetManyToManyData(IEnumerable<int> ids, bool isStage);

        string AbstractItemNetName { get; }
    }
}
