using System;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Settings;

namespace QA.DotNetCore.Engine.AbTesting.Configuration
{
    public class AbTestOptions
    {
        /// <summary>
        /// Настройки взаимодействия с QP
        /// </summary>
        public QpSettings QpSettings { get; set; }

        public AbTestingSettings AbTestingSettings { get; set; } = DefaultAbTestingSettings;

        static AbTestingSettings DefaultAbTestingSettings = new AbTestingSettings { TestsCachePeriod = new TimeSpan(0, 1, 0), ContainersCachePeriod = new TimeSpan(0, 1, 0) };
    }
}
