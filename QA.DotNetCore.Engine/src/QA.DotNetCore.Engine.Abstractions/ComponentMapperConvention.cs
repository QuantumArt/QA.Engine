namespace QA.DotNetCore.Engine.Abstractions
{
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
}
