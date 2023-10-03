using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using System.Collections.Generic;
using System.Data;

namespace QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IDictionaryItemRepository
    {
        IEnumerable<DictionaryItemPersistentData> GetAllDictionaryItems(DictionarySettings settings, int siteId, bool isStage, IDbTransaction transaction = null);
    }
}
