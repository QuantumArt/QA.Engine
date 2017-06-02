using System;

namespace QA.DotNetCore.Engine.QpData
{
    public class QpSiteStructureSettings
    {
        public bool UseCache { get; set; }
        public TimeSpan CachePeriod { get; set; }
        public string RootPageDiscriminator { get; set; }
    }
}
