using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingUrlResolver
    {
        string SanitizeUrl(string originalUrl);
        string AddCurrentTargetingValuesToUrl(string originalUrl);
        string AddTokensToUrl(string originalUrl, Dictionary<string, string> tokens);
        Dictionary<string, string> ResolveTargetingValuesFromUrl(string url);
    }
}
