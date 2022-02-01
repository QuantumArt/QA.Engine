using System.Collections.Generic;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;

namespace QA.DotNetCore.Engine.QpData.Interfaces
{
    /// <summary>
    /// Интерфейс строителя структуры сайта
    /// </summary>
    public interface IAbstractItemStorageBuilder
    {
        AbstractItemStorage Build();

        /// <summary>
        /// Формирование AbstractItem
        /// </summary>
        /// <param name="extensionContentId">Идентификатор контента расширения</param>
        /// <param name="abstractItemPersistentDatas">Идентификаторы связанный AbstractItem</param>
        /// <returns></returns>
        AbstractItem[] BuildAbstractItems(int extensionContentId, AbstractItemPersistentData[] abstractItemPersistentDatas);

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

        /// <summary>
        /// Формирование контекста
        /// </summary>
        /// <param name="extensions"></param>
        /// <returns></returns>
        void BuildContext(IDictionary<int, AbstractItemPersistentData[]> extensions);
    }
}
