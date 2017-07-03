using QA.DotNetCore.Engine.QpData.Settings;
using System;

namespace QA.DotNetCore.Engine.QpData.Configuration
{
    public class SiteStructureEngineOptions
    {
        /// <summary>
        /// Конвенция для IItemDefinitionProvider
        /// </summary>
        public ItemDefinitionConvention ItemDefinitionConvention { get; set; } = ItemDefinitionConvention.Name;

        public QpSettings QpSettings { get; set; }

        public QpSiteStructureSettings QpSiteStructureSettings { get; set; } = DefaultQpSiteStructureSettings;

        public QpSchemeCacheSettings QpSchemeCacheSettings { get; set; } = DefaultQpSchemeCacheSettings;

        public ItemDefinitionCacheSettings ItemDefinitionCacheSettings { get; set; } = DefaultItemDefinitionCacheSettings;

        static QpSiteStructureSettings DefaultQpSiteStructureSettings = new QpSiteStructureSettings { CachePeriod = new TimeSpan(0, 0, 30), RootPageDiscriminator = "root_page", UseCache = true };
        static QpSchemeCacheSettings DefaultQpSchemeCacheSettings = new QpSchemeCacheSettings { CachePeriod = new TimeSpan(0, 0, 30) };
        static ItemDefinitionCacheSettings DefaultItemDefinitionCacheSettings = new ItemDefinitionCacheSettings { CachePeriod = new TimeSpan(0, 0, 30) };
    }

    /// <summary>
    /// Конвенция об использовании ItemDefinition
    /// </summary>
    public enum ItemDefinitionConvention
    {
        /// <summary>
        /// Конвенция предполагает совпадение поля TypeName у ItemDefinition в QP и имени класса .Net, ему соответствующего
        /// </summary>
        Name,
        /// <summary>
        /// Конвенция предполагает наличие атрибута у класса .Net, в котором задаётся дискриминатор ItemDefinition из QP
        /// </summary>
        Attribute
    }
}
