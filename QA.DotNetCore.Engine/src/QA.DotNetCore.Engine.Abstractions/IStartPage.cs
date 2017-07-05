namespace QA.DotNetCore.Engine.Abstractions
{
    public interface IStartPage : IAbstractItem
    {
        string[] GetDNSBindings();
    }
}
