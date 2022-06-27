using NUnit.Framework;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure
{
    public class NunitRunSettings : IRunSettings
    {
        public string Get(string name, string defaultValue = null) => TestContext.Parameters.Get(name, defaultValue);
    }
}
