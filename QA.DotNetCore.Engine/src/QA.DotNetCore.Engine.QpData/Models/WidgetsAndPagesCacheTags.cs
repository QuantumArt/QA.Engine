using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData.Models
{
    public class WidgetsAndPagesCacheTags
    {
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
        public IDictionary<int, string> ExtensionsTags { get; set; }

        /// <summary>
        /// Получить все имеющиеся кэш теги
        /// </summary>
        public string[] AllTags
        {
            get
            {
                return (ExtensionsTags?.Values ?? Array.Empty<string>())
                    .Union(new[] {AbstractItemTag, ItemDefinitionTag})
                    .Where(x => !string.IsNullOrEmpty(x))
                    .ToArray();
            }
        }
    }
}
