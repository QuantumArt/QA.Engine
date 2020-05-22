using Microsoft.AspNetCore.Routing;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Routing
{
    public class ContentRoute : AbstractContentRoute
    {
        public ContentRoute(IControllerMapper controllerMapper,
            ITargetingFilterAccessor targetingProvider,
            IHeadUrlResolver headUrlResolver,
            IRouter target,
            string routeTemplate,
            IInlineConstraintResolver inlineConstraintResolver)
            : this(controllerMapper, targetingProvider, headUrlResolver, target, routeTemplate, null, null, null, inlineConstraintResolver)
        {
        }

        public ContentRoute(IControllerMapper controllerMapper,
            ITargetingFilterAccessor targetingProvider,
            IHeadUrlResolver headUrlResolver,
            IRouter target,
            string routeTemplate,
            RouteValueDictionary defaults,
            IDictionary<string, object> constraints,
            RouteValueDictionary dataTokens,
            IInlineConstraintResolver inlineConstraintResolver)
            : this(controllerMapper, targetingProvider, headUrlResolver, target, null, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
        }

        public ContentRoute(IControllerMapper controllerMapper,
            ITargetingFilterAccessor targetingProvider,
            IHeadUrlResolver headUrlResolver,
            IRouter target,
            string routeName,
            string routeTemplate,
            RouteValueDictionary defaults,
            IDictionary<string, object> constraints,
            RouteValueDictionary dataTokens,
            IInlineConstraintResolver inlineConstraintResolver)
            : base(controllerMapper, targetingProvider, headUrlResolver, target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
        }

        protected override bool SavePathDataInHttpContext => true;

        protected override PathFinder CreatePathFinder()
        {
            return new PathFinder();
        }
    }
}
