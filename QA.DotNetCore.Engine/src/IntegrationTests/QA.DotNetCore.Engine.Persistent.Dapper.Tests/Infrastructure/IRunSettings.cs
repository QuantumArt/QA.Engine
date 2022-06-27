namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure
{
    public interface IRunSettings
    {
        string Get(string name, string defaultValue = null);
    }
}
