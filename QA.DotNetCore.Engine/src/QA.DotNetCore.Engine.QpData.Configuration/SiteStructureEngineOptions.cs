using QA.DotNetCore.Engine.QpData.Settings;
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
        /// Строка подключения к QP
        /// </summary>
        public string QpConnectionString { get; set; }

        /// <summary>
        /// Настройки взаимодействия с QP
        /// </summary>
        public QpSettings QpSettings { get; set; }

        /// <summary>
        /// TypeFinder, позволящий регистрировать сборки
        /// </summary>
        public RegisterTypeFinder TypeFinder { get; set; } = new RegisterTypeFinder();

        public QpSiteStructureSettings QpSiteStructureSettings { get; set; } = DefaultQpSiteStructureSettings;
        public QpSchemeCacheSettings QpSchemeCacheSettings { get; set; } = DefaultQpSchemeCacheSettings;
        public ItemDefinitionCacheSettings ItemDefinitionCacheSettings { get; set; } = DefaultItemDefinitionCacheSettings;

        static QpSiteStructureSettings DefaultQpSiteStructureSettings = new QpSiteStructureSettings {
            CachePeriod = new TimeSpan(0, 20, 0),
            RootPageDiscriminator = "root_page",
            UploadUrlPlaceholder = "<%=upload_url%>",
            LoadM2mForAbstractItem = true,
            LoadAbstractItemFieldsToDetailsCollection = true
        };
        static QpSchemeCacheSettings DefaultQpSchemeCacheSettings = new QpSchemeCacheSettings { CachePeriod = new TimeSpan(0, 0, 30) };
        static ItemDefinitionCacheSettings DefaultItemDefinitionCacheSettings = new ItemDefinitionCacheSettings { CachePeriod = new TimeSpan(0, 20, 0) };
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

    /// <summary>
    /// Конвенция о способе маппинга виджетов структуры сайта и классов ViewComponent
    /// </summary>
    public enum ComponentMapperConvention
    {
        /// <summary>
        /// Конвенция предполагает, что компонент должен называться также как тип виджета
        /// </summary>
        Name,
        /// <summary>
        /// Конвенция предполагает, что компонент должен быть помечен атрибутом, в котором должен быть указан тип виджета
        /// </summary>
        Attribute
    }

    /// <summary>
    /// Конвенция о способе маппинга страниц структуры сайта и контроллеров MVC
    /// </summary>
    public enum ControllerMapperConvention
    {
        /// <summary>
        /// Конвенция предполагает, что контроллер должен называться также как тип страницы
        /// </summary>
        Name,
        /// <summary>
        /// Конвенция предполагает, что контроллер должен быть помечен атрибутом, в котором должен быть указан тип страницы
        /// </summary>
        Attribute
    }
}
