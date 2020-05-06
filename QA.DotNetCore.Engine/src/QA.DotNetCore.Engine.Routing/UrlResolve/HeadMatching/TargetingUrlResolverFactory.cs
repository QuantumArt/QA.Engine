using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching
{
    public class TargetingUrlResolverFactory
    {
        private readonly ITargetingContext _targetingContext;

        public TargetingUrlResolverFactory(ITargetingContext targetingContext)
        {
            _targetingContext = targetingContext;
        }

        public ITargetingUrlResolver Create(UrlTokenConfig urlTokenConfig)
        {
            var matcher = new HeadUrlTokenMatcher(urlTokenConfig);
            return new HeadUrlResolver(matcher, _targetingContext);
        }
    }
}
