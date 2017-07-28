using System;

namespace QA.DotNetCore.Engine.QpData.Settings
{
    /// <summary>
    /// Настройки структуры сайта, её построения и хранения
    /// </summary>
    public class QpSiteStructureSettings
    {
        /// <summary>
        /// Кешировать ли структуру сайта
        /// </summary>
        public bool UseCache { get; set; }
        /// <summary>
        /// Период хранения структуры сайта в кеше
        /// </summary>
        public TimeSpan CachePeriod { get; set; }
        /// <summary>
        /// Дискриминатор корневого элемента
        /// </summary>
        public string RootPageDiscriminator { get; set; }
        /// <summary>
        /// Плейсхолдер, который заменяет собой путь до библиотеки сайта в qp
        /// </summary>
        public string UploadUrlPlaceholder { get; set; }
        /// <summary>
        /// Загружать ли в коллекцию Details поля основного контента AbstractItem
        /// </summary>
        public bool LoadAbstractItemFieldsToDetailsCollection { get; set; }
        /// <summary>
        /// Загружать ли значения m2m-полей для основного контента AbstractItem
        /// </summary>
        public bool LoadM2mForAbstractItem { get; set; }
    }
}
