using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.QpData.Interfaces
{
    /// <summary>
    /// Интерфейс поэтапного строителя структуры сайта
    /// </summary>
    public interface IAbstractItemContextStorageBuilder
    {
        /// <summary>
        /// Формирование контекста
        /// </summary>
        /// <param name="extensions"></param>
        /// <returns></returns>
        void Init(IDictionary<int, AbstractItemPersistentData[]> extensions, bool lazyLoad);

        /// <summary>
        /// Формирование AbstractItem
        /// </summary>
        /// <param name="extensionContentId">Идентификатор контента расширения</param>
        /// <param name="abstractItemPersistentData">Идентификаторы связанный AbstractItem</param>
        /// <returns></returns>
        AbstractItem[] BuildAbstractItems(int extensionContentId, AbstractItemPersistentData[] abstractItemPersistentData, bool lazyLoad);

        /// <summary>
        /// Формирование Storage
        /// </summary>
        /// <param name="abstractItems"></param>
        /// <returns></returns>
        AbstractItemStorage BuildStorage(AbstractItem[] abstractItems);

        /// <summary>
        /// Заполняем поля иерархии Parent-Children, на основании ParentId. Заполняем VersionOf
        /// </summary>
        /// <param name="abstractItems"></param>
        void SetRelationsBetweenAbstractItems(AbstractItem[] abstractItems);
    }
}
