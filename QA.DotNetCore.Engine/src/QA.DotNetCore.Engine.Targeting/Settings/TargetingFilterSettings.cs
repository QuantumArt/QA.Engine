namespace QA.DotNetCore.Engine.Targeting.Settings
{
    public class TargetingFilterSettings
    {
        /// <summary>
        /// Название подключаемой сборки с реализацией <typeparamref name="ITargetingFiltersFactory"/> 
        /// </summary>
        public string TargetingLibrary { get; set; }
        /// <summary>
        /// Выбор реализации <typeparamref name="ITargetingFiltersFactory"/>
        /// Если это поле не задано, но приэтом указана подключается сборка, то реализация ищется в ней автоматически
        /// </summary>
        public string TargetingFiltersFactory { get; set; }
    }
}
