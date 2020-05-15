using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using QA.DotNetCore.Engine.Reflection;
using System;

namespace QA.DotNetCore.Engine.QpData.Configuration
{
    /// <summary>
    /// Настройки движка структуры сайта
    /// </summary>
    public class SiteStructureEngineOptions : SiteStructureOptions
    {
        /// <summary>
        /// Длительность кеширования ItemDefinition
        /// </summary>
        public TimeSpan ItemDefinitionCachePeriod { get; set; } = new TimeSpan(0, 20, 0);
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
