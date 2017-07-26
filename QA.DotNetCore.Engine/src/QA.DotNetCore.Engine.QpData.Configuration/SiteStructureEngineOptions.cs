using QA.DotNetCore.Engine.QpData.Settings;
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

        public QpSiteStructureSettings QpSiteStructureSettings { get; set; } = DefaultQpSiteStructureSettings;
        public QpSchemeCacheSettings QpSchemeCacheSettings { get; set; } = DefaultQpSchemeCacheSettings;
        public ItemDefinitionCacheSettings ItemDefinitionCacheSettings { get; set; } = DefaultItemDefinitionCacheSettings;
        public TypeFinderOptions TypeFinderOptions { get; set; } = DefaultTypeFinderOptions;

        static QpSiteStructureSettings DefaultQpSiteStructureSettings = new QpSiteStructureSettings { CachePeriod = new TimeSpan(0, 0, 30), RootPageDiscriminator = "root_page", UseCache = true };
        static QpSchemeCacheSettings DefaultQpSchemeCacheSettings = new QpSchemeCacheSettings { CachePeriod = new TimeSpan(0, 0, 30) };
        static ItemDefinitionCacheSettings DefaultItemDefinitionCacheSettings = new ItemDefinitionCacheSettings { CachePeriod = new TimeSpan(0, 0, 30) };
        static TypeFinderOptions DefaultTypeFinderOptions = new TypeFinderOptions { Kind = TypeFinderKind.SingleAssembly };
    }

    public class TypeFinderOptions
    {
        public TypeFinderKind Kind { get; set; }

        /// <summary>
        /// Для TypeFinderKind = SingleAssembly. Объект, определяющий сборку, в которой нужно искать
        /// </summary>
        public object Sample { get; set; }
    }

    /// <summary>
    /// Виды реализаций ITypeFinder
    /// </summary>
    public enum TypeFinderKind
    {
        SingleAssembly
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
