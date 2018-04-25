using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Routing.UrlResolve
{
    public class UrlTokenResolverFactory
    {
        private readonly ITargetingContext _targetingContext;

        public UrlTokenResolverFactory(ITargetingContext targetingContext)
        {
            _targetingContext = targetingContext;
        }

        public ITargetingUrlResolver Create(UrlTokenConfig urlTokenConfig)
        {
            var matcher = new UrlTokenMatcher(urlTokenConfig);
            return new UrlTokenResolver(matcher, _targetingContext);
        }
    }
}
