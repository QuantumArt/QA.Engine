namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingContext
    {
        object GetPrimaryTargetingValue(string key);
        object GetTargetingValue(string key);
        string[] GetTargetingKeys();
    }
}
