using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingUrlResolver
    {
        string AddTokensToUrl(string originalUrl);
        Dictionary<string, string> ResolveTargetingValuesFromUrl(string url);
    }
}
