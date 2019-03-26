using Microsoft.AspNetCore.Routing;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Routing
{
    /// <summary>
    /// Отличие от ContentRoute в том, что он считает роут сматченным как только находит жадную страницу при траверсе структуры сайта
    /// </summary>
    public class GreedyContentRoute : AbstractContentRoute
    {
        public GreedyContentRoute(IControllerMapper controllerMapper, ITargetingFilterAccessor targetingProvider, IRouter target, string routeTemplate, IInlineConstraintResolver inlineConstraintResolver, IOnScreenContextProvider onScreenContextProvider)
            : this(controllerMapper, targetingProvider, target, routeTemplate, null, null, null, inlineConstraintResolver, onScreenContextProvider)
        {
        }

        public GreedyContentRoute(IControllerMapper controllerMapper, ITargetingFilterAccessor targetingProvider, IRouter target, string routeTemplate, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver, IOnScreenContextProvider onScreenContextProvider)
            : this(controllerMapper, targetingProvider, target, null, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver, onScreenContextProvider)
        {
        }

        public GreedyContentRoute(IControllerMapper controllerMapper, ITargetingFilterAccessor targetingProvider, IRouter target, string routeName, string routeTemplate, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver, IOnScreenContextProvider onScreenContextProvider)
            : base(controllerMapper, targetingProvider, target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver, onScreenContextProvider)
        {
        }

        protected override bool SavePathDataInHttpContext => false;

        protected override PathFinder CreatePathFinder()
        {
            //добавляем условие остановки в траверс структуры сайта: когда доходим до страницы, которой соответствует контроллер, переданный в Defaults
            return new PathFinder(ai => Defaults.ContainsKey("controller") ? Defaults["controller"].ToString() == ControllerMapper.Map(ai) : false);
        }
    }
}
