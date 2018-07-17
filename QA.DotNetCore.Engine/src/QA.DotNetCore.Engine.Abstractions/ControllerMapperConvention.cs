namespace QA.DotNetCore.Engine.Abstractions
{
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
