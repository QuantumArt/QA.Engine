using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using System;

namespace QA.DotNetCore.Engine.AbTesting.Configuration
{
    public class AbTestOptions
    {
        /// <summary>
        /// Настройки взаимодействия с QP
        /// </summary>
        public QpSettings QpSettings { get; set; }

        public AbTestingSettings AbTestingSettings { get; set; } = DefaultAbTestingSettings;

        static readonly AbTestingSettings DefaultAbTestingSettings = new AbTestingSettings { TestsCachePeriod = new TimeSpan(0, 1, 0), ContainersCachePeriod = new TimeSpan(0, 1, 0) };
    }
}
