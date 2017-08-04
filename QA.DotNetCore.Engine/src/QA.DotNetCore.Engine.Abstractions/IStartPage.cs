namespace QA.DotNetCore.Engine.Abstractions
{
    public interface IStartPage : IAbstractPage
    {
        string[] GetDNSBindings();
    }
}
