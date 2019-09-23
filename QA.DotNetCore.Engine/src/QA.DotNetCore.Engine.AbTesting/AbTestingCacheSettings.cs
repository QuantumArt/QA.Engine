using System;

namespace QA.DotNetCore.Engine.AbTesting
{
    public class AbTestingCacheSettings
    {
        /// <summary>
        /// Длительность кеширования описаний тестов
        /// </summary>
        public TimeSpan TestsCachePeriod { get; set; }
        /// <summary>
        /// Длительность кеширования контейнеров тестов 
        /// </summary>
        public TimeSpan ContainersCachePeriod { get; set; }
    }
}
