using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;
using System.Data;

namespace  QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IAbstractItemRepository
    {
        IEnumerable<AbstractItemPersistentData> GetPlainAllAbstractItems(int siteId, bool isStage, IDbTransaction transaction = null);

        IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(int extensionContentId, IEnumerable<int> ids, bool loadAbstractItemFields, bool isStage, IDbTransaction transaction = null);

        IDictionary<int, M2mRelations> GetManyToManyData(IEnumerable<int> ids, bool isStage, IDbTransaction transaction = null);

        string AbstractItemNetName { get; }
    }
}
