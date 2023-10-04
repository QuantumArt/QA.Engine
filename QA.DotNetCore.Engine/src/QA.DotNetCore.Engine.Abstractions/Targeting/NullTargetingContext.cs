namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public sealed class NullTargetingContext : ITargetingContext
    {
        public object GetPrimaryTargetingValue(string key) => new string[0];
        public string[] GetTargetingKeys() => new string[0];
        public object GetTargetingValue(string key) => null;
    }
}
