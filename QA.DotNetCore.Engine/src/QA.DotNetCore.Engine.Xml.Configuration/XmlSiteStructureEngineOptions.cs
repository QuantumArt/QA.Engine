using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Reflection;

namespace QA.DotNetCore.Engine.Xml.Configuration
{
    /// <summary>
    /// Настройки движка структуры сайта
    /// </summary>
    public class XmlSiteStructureEngineOptions
    {
        /// <summary>
        /// Конвенция для маппинга компонентов
        /// </summary>
        public ComponentMapperConvention ComponentMapperConvention { get; set; } = ComponentMapperConvention.Name;

        /// <summary>
        /// Конвенция для маппинга контроллеров
        /// </summary>
        public ControllerMapperConvention ControllerMapperConvention { get; set; } = ControllerMapperConvention.Name;

        /// <summary>
        /// Настройки xml-файла
        /// </summary>
        public XmlStorageSettings Settings { get; set; } = new XmlStorageSettings();

        /// <summary>
        /// TypeFinder, позволящий регистрировать сборки
        /// </summary>
        public RegisterTypeFinder TypeFinder { get; set; } = new RegisterTypeFinder();
    }
}
