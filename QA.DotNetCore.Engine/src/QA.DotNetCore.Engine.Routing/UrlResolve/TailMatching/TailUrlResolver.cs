using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching
{
    public interface ITailUrlResolver
    {
        IDictionary<string, object> ResolveRouteValues(string tail, string controllerName);
    }

    public class TailUrlResolver : ITailUrlResolver
    {
        public TailUrlResolver(UrlTokenConfig urlTokenConfig)
        {
            UrlTokenConfig = urlTokenConfig;
        }

        public UrlTokenConfig UrlTokenConfig { get; }

        public IDictionary<string, object> ResolveRouteValues(string tail, string controllerName)
        {
            var controllerKey = UrlTokenConfig.TailPatternsByControllers?.Keys?.FirstOrDefault(k => k.Equals(controllerName, StringComparison.InvariantCultureIgnoreCase));
            var patterns = controllerKey == null ?
                new List<TailUrlMatchingPattern>() :
                UrlTokenConfig.TailPatternsByControllers[controllerKey];

            foreach (var pattern in patterns)
            {
                var matchResult = pattern.Match(tail);
                if (matchResult.IsMatch)
                {
                    return matchResult.Values.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value);
                }
            }

            if (UrlTokenConfig.DefaultTailPattern != null)
            {
                var defaultPatternMatch = UrlTokenConfig.DefaultTailPattern.Match(tail);
                if (defaultPatternMatch.IsMatch)
                    return defaultPatternMatch.Values.ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value);
            }
            return new Dictionary<string, object>();
        }
    }
}
