using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Routing.Mappers
{
    /// <summary>
    /// Реализация IControllerMapper, основанная на соответствии имени типа страницы и имени конроллера
    /// </summary>
    public class NameConventionalControllerMapper : IControllerMapper
    {
        public string Map(IAbstractItem page)
        {
            //считаем, что конроллер должен называться также как тип страницы
            return page.GetType().Name;
        }
    }
}
