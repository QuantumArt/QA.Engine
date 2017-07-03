using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Routing.Configuration
{
    public static class RouteBuilderExtensions
    {
        public static IRouteBuilder MapContentRoute(this IRouteBuilder routes, string name, string template)
        {
            return MapContentRoute(routes, name, template, new RouteValueDictionary(null), new RouteValueDictionary(null), new RouteValueDictionary(null));
        }

        public static IRouteBuilder MapContentRoute(this IRouteBuilder routes, string name, string template, RouteValueDictionary defaults)
        {
            return MapContentRoute(routes, name, template, defaults, new RouteValueDictionary(null), new RouteValueDictionary(null));
        }

        public static IRouteBuilder MapContentRoute(this IRouteBuilder routes, string name, string template, RouteValueDictionary defaults, IDictionary<string, object> constraints)
        {
            return MapContentRoute(routes, name, template, defaults, constraints, new RouteValueDictionary(null));
        }

        public static IRouteBuilder MapContentRoute(this IRouteBuilder routes, string name, string template, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens)
        {
            IInlineConstraintResolver requiredService = routes.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();
            IControllerMapper controllerMapper = routes.ServiceProvider.GetRequiredService<IControllerMapper>();
            ITargetingFilterAccessor targetingAccessor = routes.ServiceProvider.GetService<ITargetingFilterAccessor>();

            routes.Routes.Add(new ContentRoute(controllerMapper, targetingAccessor, routes.DefaultHandler, name, template,
                    defaults,
                    constraints,
                    dataTokens,
                    requiredService));

            return routes;
        }
    }
}
