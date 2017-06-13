namespace QA.DotNetCore.Engine.Abstractions
{
    /// <summary>
    /// Правила соответствия элемента структуры сайта и контроллера, обрабатывающего запрос по нему
    /// </summary>
    public interface IControllerMapper
    {
        string Map(IAbstractItem page);
    }
}
