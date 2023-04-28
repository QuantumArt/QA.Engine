using System;

namespace QA.DotNetCore.Engine.QpData.Settings
{
    /// <summary>
    /// Настройки кеширования при построении структуры сайта QP 
    /// </summary>
    public class QpSiteStructureCacheSettings
    {
        /// <summary>
        /// Длительность кеширования схемы QP (такие вещи как таблица SITE, CONTENT_ATTRIBUTE итп)
        /// </summary>
        public TimeSpan QpSchemeCachePeriod { get; set; }

        /// <summary>
        /// Длительность кеширования ItemDefinition
        /// </summary>
        public TimeSpan ItemDefinitionCachePeriod { get; set; }

        /// <summary>
        /// Длительность кеширования уже построенной структуры сайта
        /// </summary>
        public TimeSpan SiteStructureCachePeriod { get; set; }

        /// <summary>
        /// Тип кэширования
        /// </summary>
        public SiteStructureCachingType SiteStructureCachingType { get; set; }
    }
}
