using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Routing
{
    public class UrlTokenResolver : ITargetingUrlResolver
    {
        private readonly IUrlTokenMatcher _urlTokenMatcher;
        private readonly ITargetingContext _targetingContext;

        public UrlTokenResolver(IUrlTokenMatcher urlTokenMatcher,
            ITargetingContext targetingContext)
        {
            _urlTokenMatcher = urlTokenMatcher;
            _targetingContext = targetingContext;
        }

        public string AddCurrentTargetingValuesToUrl(string originalUrl)
        {
            var allCurrentTargetingTokens = _targetingContext.GetTargetingKeys().ToDictionary(k => k, k => _targetingContext.GetTargetingValue(k).ToString());
            return AddTokensToUrl(originalUrl, allCurrentTargetingTokens);
        }

        public virtual string AddTokensToUrl(string originalUrl, Dictionary<string, string> tokenValues)
        {
            return _urlTokenMatcher.ReplaceTokens(originalUrl, tokenValues, _targetingContext);
        }

        public virtual Dictionary<string, string> ResolveTargetingValuesFromUrl(string url)
        {
            var m = _urlTokenMatcher.Match(url, _targetingContext);
            return m.TokenValues;
        }

        public virtual string SanitizeUrl(string originalUrl)
        {
            var m = _urlTokenMatcher.Match(originalUrl, _targetingContext);
            if (m.IsMatch)
            {
                return m.SanitizedUrl;
            }
            return originalUrl;
        }
    }
}
