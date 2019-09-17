using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using System;

namespace QA.DotNetCore.Engine.AbTesting.Configuration
{
    /// <summary>
    /// Настройки движка аб-тестов
    /// </summary>
    public class AbTestOptions
    {
        /// <summary>
        /// Строка подключения к базе QP
        /// </summary>
        public string QpConnectionString { get; set; }
        /// <summary>
        /// Тип базы (mssql/postgres)
        /// </summary>
        public string QpDatabaseType { get; set; } = "mssql";
        /// <summary>
        /// Id сайта в QP
        /// </summary>
        public int SiteId { get; set; }
        /// <summary>
        /// Режим Stage
        /// </summary>
        public bool IsStage { get; set; }


        /// <summary>
        /// Длительность кеширования описаний тестов
        /// </summary>
        public TimeSpan TestsCachePeriod { get; set; } = new TimeSpan(0, 1, 0);
        /// <summary>
        /// Длительность кеширования контейнеров тестов 
        /// </summary>
        public TimeSpan ContainersCachePeriod { get; set; } = new TimeSpan(0, 1, 0);


        /// <summary>
        /// Использовать глобальные настройки для взаимодействия с QP
        /// </summary>
        /// <param name="qpSettings"></param>
        public void UseQpSettings(QpSettings qpSettings)
        {
            QpConnectionString = qpSettings.ConnectionString;
            QpDatabaseType = qpSettings.DatabaseType;
            SiteId = qpSettings.SiteId;
            IsStage = qpSettings.IsStage;
        }
    }
}
