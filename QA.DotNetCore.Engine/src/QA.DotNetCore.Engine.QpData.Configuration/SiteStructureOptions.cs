using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using System;

namespace QA.DotNetCore.Engine.QpData.Configuration
{
    public class SiteStructureOptions
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
        /// Дискриминатор корневого элемента
        /// </summary>
        public string RootPageDiscriminator { get; set; } = "root_page";
        /// <summary>
        /// Плейсхолдер, который заменяет собой путь до библиотеки сайта в qp
        /// </summary>
        public string UploadUrlPlaceholder { get; set; } = "<%=upload_url%>";
        /// <summary>
        /// Загружать ли в коллекцию Details поля основного контента AbstractItem
        /// </summary>
        public bool LoadAbstractItemFieldsToDetailsCollection { get; set; } = true;

        /// <summary>
        /// Длительность кеширования уже построенной структуры сайта
        /// </summary>
        public TimeSpan SiteStructureCachePeriod { get; set; } = new TimeSpan(1, 0, 0);

        public SiteStructureCachingType SiteStructureCachingType { get; set; } = SiteStructureCachingType.Simple;
        /// <summary>
        /// Длительность кеширования схемы QP (такие вещи как таблица SITE, CONTENT_ATTRIBUTE итп)
        /// </summary>
        public TimeSpan QpSchemeCachePeriod { get; set; } = new TimeSpan(0, 10, 0);
        /// <summary>
        /// Длительность кеширования ItemDefinition
        /// </summary>
        public TimeSpan ItemDefinitionCachePeriod { get; set; } = new TimeSpan(1, 0, 0);

        /// <summary>
        /// Таймаут ожидания потоков, при получении <see cref="AbstractItemStorage"/> из кеша
        /// </summary>
        public TimeSpan CacheFetchTimeoutAbstractItemStorage { get; set; } = new TimeSpan(0, 2, 0);

        /// <summary>
        /// Использовать глобальные настройки для взаимодействия с QP
        /// </summary>
        /// <param name="qpSettings"></param>
        public void UseQpSettings(QpSettings qpSettings)
        {
            if (qpSettings is null)
            {
                throw new ArgumentNullException(nameof(qpSettings));
            }

            SiteId = qpSettings.SiteId;
            IsStage = qpSettings.IsStage;
            QpConnectionString = qpSettings.ConnectionString;
            QpDatabaseType = qpSettings.DatabaseType;
        }
    }
}
