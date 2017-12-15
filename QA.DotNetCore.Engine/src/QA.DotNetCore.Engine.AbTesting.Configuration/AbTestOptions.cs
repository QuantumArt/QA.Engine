using System;

namespace QA.DotNetCore.Engine.AbTesting.Configuration
{
    public class AbTestOptions
    {
        public string QpConnectionString { get; set; }

        public AbTestingSettings AbTestingSettings { get; set; } = DefaultAbTestingSettings;

        static AbTestingSettings DefaultAbTestingSettings = new AbTestingSettings { TestsCachePeriod = new TimeSpan(0, 1, 0), ContainersCachePeriod = new TimeSpan(0, 1, 0) };
    }
}
