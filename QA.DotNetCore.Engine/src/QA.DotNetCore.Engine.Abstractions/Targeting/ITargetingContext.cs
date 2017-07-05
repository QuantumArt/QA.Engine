namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingContext
    {
        object GetTargetingValue(string key);
    }
}
