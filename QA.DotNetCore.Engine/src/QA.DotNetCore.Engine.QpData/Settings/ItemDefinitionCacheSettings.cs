using System;

namespace QA.DotNetCore.Engine.QpData.Settings
{
    /// <summary>
    /// Настройки кеширования ItemDefinition
    /// </summary>
    public class ItemDefinitionCacheSettings
    {
        public TimeSpan CachePeriod { get; set; }
    }
}
