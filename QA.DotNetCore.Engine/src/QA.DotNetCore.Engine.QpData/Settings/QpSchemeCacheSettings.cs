using System;

namespace QA.DotNetCore.Engine.QpData.Settings
{
    /// <summary>
    /// Настройки кеширования схемы QP (такие вещи как таблица SITE, CONTENT_ATTRIBUTE итп)
    /// </summary>
    public class QpSchemeCacheSettings
    {
        public TimeSpan CachePeriod { get; set; }
    }
}
