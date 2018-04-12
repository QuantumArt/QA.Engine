using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Abstractions
{
    public interface IStartPage : IAbstractPage
    {
        string[] GetDNSBindings();
        ITargetingUrlResolver GetUrlResolver();
    }
}
