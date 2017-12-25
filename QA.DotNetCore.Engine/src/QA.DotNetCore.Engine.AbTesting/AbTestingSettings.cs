using System;
using System.Collections.Generic;
using System.Text;

namespace QA.DotNetCore.Engine.AbTesting
{
    public class AbTestingSettings
    {
        public int SiteId { get; set; }
        public bool IsStage { get; set; }
        public TimeSpan TestsCachePeriod { get; set; }
        public TimeSpan ContainersCachePeriod { get; set; }
    }
}
