namespace QA.DotNetCore.Engine.Abstractions
{
    public interface IAbstractPage : IAbstractItem
    {
        bool IsVisible { get; }
    }
}
