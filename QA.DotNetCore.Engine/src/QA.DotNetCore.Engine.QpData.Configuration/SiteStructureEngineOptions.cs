using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using QA.DotNetCore.Engine.Reflection;
using System;

namespace QA.DotNetCore.Engine.QpData.Configuration
{
    /// <summary>
    /// Настройки движка структуры сайта
    /// </summary>
    public class SiteStructureEngineOptions
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
        /// Загружать ли значения m2m-полей для основного контента AbstractItem
        /// </summary>
        public bool LoadM2mForAbstractItem { get; set; } = true;
        /// <summary>
        /// Загружать ли значения m2m-полей для всех контентов-расширений
        /// </summary>
        public bool LoadM2mForAllExtensions { get; set; } = false;

        /// <summary>
        /// Длительность кеширования схемы QP (такие вещи как таблица SITE, CONTENT_ATTRIBUTE итп)
        /// </summary>
        public TimeSpan QpSchemeCachePeriod { get; set; } = new TimeSpan(0, 0, 30);
        /// <summary>
        /// Длительность кеширования ItemDefinition
        /// </summary>
        public TimeSpan ItemDefinitionCachePeriod { get; set; } = new TimeSpan(0, 20, 0);
        /// <summary>
        /// Длительность кеширования уже построенной структуры сайта
        /// </summary>
        public TimeSpan SiteStructureCachePeriod { get; set; } = new TimeSpan(0, 20, 0);


        /// <summary>
        /// Конвенция для IItemDefinitionProvider
        /// </summary>
        public ItemDefinitionConvention ItemDefinitionConvention { get; set; } = ItemDefinitionConvention.Name;
        /// <summary>
        /// Конвенция для маппинга компонентов
        /// </summary>
        public ComponentMapperConvention ComponentMapperConvention { get; set; } = ComponentMapperConvention.Name;
        /// <summary>
        /// Конвенция для маппинга контроллеров
        /// </summary>
        public ControllerMapperConvention ControllerMapperConvention { get; set; } = ControllerMapperConvention.Name;
        /// <summary>
        /// TypeFinder, позволящий регистрировать сборки для инстанцирования страниц, виджетов, контроллеров и view-компонентов
        /// </summary>
        public RegisterTypeFinder TypeFinder { get; set; } = new RegisterTypeFinder();

        /// <summary>
        /// Использовать глобальные настройки для взаимодействия с QP
        /// </summary>
        /// <param name="qpSettings"></param>
        public void UseQpSettings(QpSettings qpSettings)
        {
            SiteId = qpSettings.SiteId;
            IsStage = qpSettings.IsStage;
            QpConnectionString = qpSettings.ConnectionString;
            QpDatabaseType = qpSettings.DatabaseType;
        }
    }

    /// <summary>
    /// Конвенция об использовании ItemDefinition
    /// </summary>
    public enum ItemDefinitionConvention
    {
        /// <summary>
        /// Конвенция предполагает совпадение поля TypeName у ItemDefinition в QP и имени класса .Net, ему соответствующего
        /// </summary>
        Name,
        /// <summary>
        /// Конвенция предполагает наличие атрибута у класса .Net, в котором задаётся дискриминатор ItemDefinition из QP
        /// </summary>
        Attribute
    }
}
