using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using System.Linq;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.Targeting
{
    public class TargetingUrlTransformator : ITargetingUrlTransformator
    {
        private readonly ITargetingContext _targetingContext;
        private readonly IHeadUrlResolver _urlResolver;

        public TargetingUrlTransformator(ITargetingContext targetingContext, IHeadUrlResolver urlResolver)
        {
            _targetingContext = targetingContext;
            _urlResolver = urlResolver;
        }

        public virtual string AddCurrentTargetingValuesToUrl(string originalUrl)
        {
            var allCurrentTargetingTokens = _targetingContext
                .GetTargetingKeys()
                .ToDictionary(k => k, k => _targetingContext.GetPrimaryTargetingValue(k).ToString());

            return _urlResolver.AddTokensToUrl(originalUrl, allCurrentTargetingTokens);
        }
    }
}
