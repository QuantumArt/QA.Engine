using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;
using System.Data;

namespace  QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IAbstractItemRepository
    {
        IEnumerable<AbstractItemPersistentData> GetPlainAllAbstractItems(int siteId, bool isStage, IDbTransaction transaction = null);

        IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(int extensionContentId,
            IEnumerable<int> ids, ContentPersistentData baseContent,
            bool loadAbstractItemFields, bool isStage, IDbTransaction transaction = null);

        IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionlessData(IEnumerable<int> ids,
            ContentPersistentData baseContent,
            bool isStage, IDbTransaction transaction = null);

        IDictionary<int, M2mRelations> GetManyToManyData(IEnumerable<int> ids, bool isStage, IDbTransaction transaction = null);

        /// <summary>
        /// Получить Content_item_id расширений
        /// </summary>
        /// <param name="extensionsContents">Словарь ID контента расширений и использующия их коллекция AbstractItems</param>
        /// <returns></returns>
        IEnumerable<int> GetAbstractItemExtensionIds(IDictionary<int, IEnumerable<int>> extensionsContents,
            bool isStage, IDbTransaction transaction = null);
    }
}
