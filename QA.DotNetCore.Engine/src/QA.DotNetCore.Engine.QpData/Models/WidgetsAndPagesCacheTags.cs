using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData.Models
{
    public class WidgetsAndPagesCacheTags
    {
        private IDictionary<int, string> _extensionsTags = new Dictionary<int, string>(0);

        /// <summary>
        /// Тег для контента AbstractItem
        /// </summary>
        public string AbstractItemTag { get; set; }

        /// <summary>
        /// Тег для контента ItemDefinition
        /// </summary>
        public string ItemDefinitionTag { get; set; }

        /// <summary>
        /// Теги для контентов расширений
        /// </summary>
        public IDictionary<int, string> ExtensionsTags
        {
            get => _extensionsTags;
            set => _extensionsTags = value ?? new Dictionary<int, string>(0);
        }

        /// <summary>
        /// Получить все имеющиеся кэш теги
        /// </summary>
        public string[] AllTags
        {
            get
            {
                return ExtensionsTags.Values
                    .Union(new[] { AbstractItemTag, ItemDefinitionTag })
                    .Where(tag => !string.IsNullOrEmpty(tag))
                    .ToArray();
            }
        }
    }
}
