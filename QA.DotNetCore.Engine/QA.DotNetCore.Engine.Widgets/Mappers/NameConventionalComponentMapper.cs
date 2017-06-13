using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Widgets.Mappers
{
    /// <summary>
    /// Реализация IControllerMapper, основанная на соответствии имени типа виджета и имени компонента
    /// </summary>
    public class NameConventionalComponentMapper : IComponentMapper
    {
        public string Map(IAbstractItem widget)
        {
            //считаем, что компонент должен называться также как тип виджета
            return widget.GetType().Name;
        }
    }
}
