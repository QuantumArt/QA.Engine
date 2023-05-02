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

        public IDictionary<int, AbstractItemExtensionCollection> GetExtensionData(int key)
        {
            var result = (ExtensionData != null) ? ExtensionData[key] : LazyExtensionData[key].Value;
            return result ?? new Dictionary<int, AbstractItemExtensionCollection>();
        }

        /// <summary>
        /// Словарь, где ключ - это Content Id расширения, в значение - это статьи расширения, где поля раскиданы по ключам
        /// </summary>
        public IDictionary<int, Lazy<IDictionary<int, AbstractItemExtensionCollection>>> LazyExtensionData { get; set; }
        
        /// <summary>
        /// Словарь, где ключ - это Content Id расширения, в значение - это статьи расширения, где поля раскиданы по ключам
        /// </summary>
        
        public IDictionary<int, IDictionary<int, AbstractItemExtensionCollection>> ExtensionData { get; set; }

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
        public bool NeedLoadM2MInAbstractItem { get; set; }

        /// <summary>
        /// M2M контентов расширений по content_item_id
        /// </summary>
        public IDictionary<int, M2MRelations> ExtensionsM2MData { get; set; }

        /// <summary>
        /// M2M у AbstractItems по content_item_id
        /// </summary>
        public IDictionary<int, M2MRelations> AbstractItemsM2MData { get; set; }
        
        public IDictionary<int, Dictionary<string, int>> M2MFields { get; set; }

        public string LogId { get; }
    }
}
