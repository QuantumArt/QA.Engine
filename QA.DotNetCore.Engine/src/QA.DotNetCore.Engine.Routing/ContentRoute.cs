using Microsoft.AspNetCore.Routing;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Routing
{
    public class ContentRoute : AbstractContentRoute
    {
        public ContentRoute(IControllerMapper controllerMapper, ITargetingFilterAccessor targetingProvider, IRouter target, string routeTemplate, IInlineConstraintResolver inlineConstraintResolver, IOnScreenContextProvider onScreenContextProvider)
            : this(controllerMapper, targetingProvider, target, routeTemplate, null, null, null, inlineConstraintResolver, onScreenContextProvider)
        {
        }

        public ContentRoute(IControllerMapper controllerMapper, ITargetingFilterAccessor targetingProvider, IRouter target, string routeTemplate, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver, IOnScreenContextProvider onScreenContextProvider)
            : this(controllerMapper, targetingProvider, target, null, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver, onScreenContextProvider)
        {
        }

        public ContentRoute(IControllerMapper controllerMapper, ITargetingFilterAccessor targetingProvider, IRouter target, string routeName, string routeTemplate, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver, IOnScreenContextProvider onScreenContextProvider)
            : base(controllerMapper, targetingProvider, target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver, onScreenContextProvider)
        {
        }

        protected override bool SavePathDataInHttpContext => true;

        protected override PathFinder CreatePathFinder()
        {
            return new PathFinder();
        }
    }
}
