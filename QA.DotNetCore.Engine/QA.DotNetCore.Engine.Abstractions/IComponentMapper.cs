namespace QA.DotNetCore.Engine.Abstractions
{
    /// <summary>
    /// Правила соответствия виджета структуры сайта и компонента, отвечающего за его рендеринг
    /// </summary>
    public interface IComponentMapper
    {
        string Map(IAbstractItem widget);
    }
}
