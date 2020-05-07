namespace QA.DotNetCore.Engine.QpData.Settings
{
    /// <summary>
    /// Настройки, нужные для построения структуры сайта из QP
    /// </summary>
    public class QpSiteStructureBuildSettings
    {
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
        /// <summary>
        /// Загружать ли значения m2m-полей для всех контентов-расширений
        /// </summary>
        public bool LoadM2mForAllExtensions { get; set; }
    }
}
