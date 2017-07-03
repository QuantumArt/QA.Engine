using System;

namespace QA.DotNetCore.Engine.QpData.Settings
{
    /// <summary>
    /// Настройки структуры сайта, её построения и хранения
    /// </summary>
    public class QpSiteStructureSettings
    {
        public bool UseCache { get; set; }
        public TimeSpan CachePeriod { get; set; }
        public string RootPageDiscriminator { get; set; }
    }
}
