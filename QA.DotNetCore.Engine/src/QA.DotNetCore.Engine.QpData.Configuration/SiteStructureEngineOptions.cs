using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using QA.DotNetCore.Engine.Reflection;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching;
using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.QpData.Configuration
{
    /// <summary>
    /// Настройки движка структуры сайта
    /// </summary>
    public class SiteStructureEngineOptions : SiteStructureOptions
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
        /// TypeFinder, позволящий регистрировать сборки для инстанцирования страниц, виджетов, контроллеров и view-компонентов
        /// </summary>
        public RegisterTypeFinder TypeFinder { get; set; } = new RegisterTypeFinder();


        /// <summary>
        /// ТОЛЬКО для Endpoint routing (MapSiteStructureControllerRoute). Шаблон "хвоста" урла по умолчанию для всех контроллеров
        /// </summary>
        public TailUrlMatchingPattern DefaultUrlTailPattern { get; set; } = new TailUrlMatchingPattern { Pattern = "{action=Index}/{id?}" };
        /// <summary>
        /// ТОЛЬКО для Endpoint routing (MapSiteStructureControllerRoute). Шаблоны "хвоста" урла, заданные индивидуально для каждого контроллера
        /// </summary>
        public Dictionary<string, List<TailUrlMatchingPattern>> UrlTailPatternsByControllers { get; set; }

        /// <summary>
        /// Шаблоны "головы" урла
        /// </summary>
        public List<HeadUrlMatchingPattern> UrlHeadPatterns { get; set; }
        /// <summary>
        /// Зарегистрировать поставщика возможных значений токенов из UrlHeadPatterns.
        /// Если используется, к примеру, {region} и известен список возможных значений, - можно зарегистрировать их для более корректного разбора урла на сегменты.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterUrlHeadTokenPossibleValues<T>() where T : IHeadTokenPossibleValuesProvider
        {
            headTokenPossibleValuesProviders.Add(typeof(T));
        }

        private readonly List<Type> headTokenPossibleValuesProviders = new List<Type>();

        public IReadOnlyList<Type> HeadTokenPossibleValuesProviders { get { return headTokenPossibleValuesProviders.AsReadOnly(); } }

        /// <summary>
        /// Настройки справочников таргетинга
        /// </summary>
        public List<DictionarySettings> DictionarySettings { get; set; }
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
