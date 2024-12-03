using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;
using System.Data;

namespace QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IAbstractItemRepository
    {
        IEnumerable<AbstractItemPersistentData> GetPlainAllAbstractItems(int siteId, bool isStage,
            IDbTransaction transaction = null);

        IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionData(
            int extensionContentId,
            ContentPersistentData baseContent,
            bool loadAbstractItemFields,
            bool isStage,
            IDbTransaction transaction = null);

        IDictionary<int, AbstractItemExtensionCollection> GetAbstractItemExtensionlessData(
            int[] ids,
            ContentPersistentData baseContent,
            bool isStage,
            IDbTransaction transaction = null);

        IDictionary<int, M2MRelations> GetManyToManyData(int[] itemIds, bool isStage, IDbTransaction transaction = null);

        IDictionary<int, M2MRelations> GetManyToManyDataByContents(
            int[] contentIds,
            bool isStage,
            IDbTransaction transaction = null);

        /// <summary>
        /// Получить id статей-расширений
        /// </summary>
        /// <param name="extensionContentIds">Список ID контентов расширений.</param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        IEnumerable<int> GetAbstractItemExtensionIds(IReadOnlyCollection<int> extensionContentIds,
            IDbTransaction transaction = null);
    }
}
