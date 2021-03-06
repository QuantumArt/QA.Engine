namespace QA.DotNetCore.Engine.Abstractions.OnScreen
{
    public class OnScreenContext
    {
        /// <summary>
        /// Доступен ли OnScreen
        /// </summary>
        public bool Enabled => Features != OnScreenFeatures.None;

        /// <summary>
        /// Доступные фичи OnScreen
        /// </summary>
        public OnScreenFeatures Features { get; set; }

        /// <summary>
        /// Текущий пользователь
        /// </summary>
        public OnScreenUser User { get; set; }

        /// <summary>
        /// Проверка, доступна ли фича
        /// </summary>
        /// <param name="feature">фича OnScreen</param>
        /// <returns></returns>
        public bool HasFeature(OnScreenFeatures feature)
        {
            return (Features & feature) > 0;
        }
        /// <summary>
        /// Переопределенное для режима onscreen значение isStage для получения данных по АБ-тестам
        /// </summary>
        public bool? AbtestsIsStageOverrided { get; set; }

        /// <summary>
        /// Типы виджетов, которые надо игнорировать в режиме onscreen (не подсвечивать, обрамлять рамками итд)
        /// </summary>
        public string[] SkipWidgetTypes { get; set; }

        /// <summary>
        /// Id страницы
        /// </summary>
        public int? PageId { get; set; }

        public string CustomerCode { get; set; }
    }
}
