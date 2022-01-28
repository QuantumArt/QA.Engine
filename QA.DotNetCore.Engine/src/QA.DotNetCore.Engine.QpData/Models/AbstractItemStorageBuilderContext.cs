using System;
using System.Collections.Generic;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;

namespace QA.DotNetCore.Engine.QpData.Models
{
    public class AbstractItemStorageBuilderContext
    {
        public AbstractItemStorageBuilderContext()
        {
            LogId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Словарь, где ключ - это Content Id расширения, в значение - это статьи расширения, где поля раскиданы по ключам
        /// </summary>
        public IDictionary<int, Lazy<IDictionary<int, AbstractItemExtensionCollection>>> ExtensionDataLazy { get; set; }

        /// <summary>
        /// Информация обо всех контентах-расширениях, которые используются
        /// </summary>
        public IDictionary<int, ContentPersistentData> ExtensionContents { get; set; }

        /// <summary>
        /// Контент QP. (Таблица CONTENT)
        /// </summary>
        public ContentPersistentData BaseContent { get; set; }

        /// <summary>
        /// Флаг, определяющий необходимость подгрузки M2M для AbstractItem
        /// </summary>
        public bool NeedLoadM2mInAbstractItem { get; set; }

        /// <summary>
        /// M2M контентов расширений по content_item_id
        /// </summary>
        public IDictionary<int, M2mRelations> ExtensionsM2MData { get; set; }

        /// <summary>
        /// M2M у AbstractItems по content_item_id
        /// </summary>
        public IDictionary<int, M2mRelations> AbstractItemsM2MData { get; set; }

        public string LogId { get; }
    }
}
