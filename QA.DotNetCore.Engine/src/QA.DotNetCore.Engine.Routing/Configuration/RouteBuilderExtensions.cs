using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Routing.Configuration
{
    public static class RouteBuilderExtensions
    {
        public static IRouteBuilder MapContentRoute(this IRouteBuilder routes, string name, string template)
        {
            return MapContentRoute(routes, name, template, null, null, null);
        }

        public static IRouteBuilder MapContentRoute(this IRouteBuilder routes, string name, string template, object defaults)
        {
            return MapContentRoute(routes, name, template, defaults, null, null);
        }

        public static IRouteBuilder MapContentRoute(this IRouteBuilder routes, string name, string template, object defaults, object constraints)
        {
            return MapContentRoute(routes, name, template, defaults, constraints, null);
        }

        public static IRouteBuilder MapContentRoute(this IRouteBuilder routes, string name, string template, object defaults, object constraints, object dataTokens)
        {
            IInlineConstraintResolver requiredService = routes.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();
            IControllerMapper controllerMapper = routes.ServiceProvider.GetRequiredService<IControllerMapper>();
            ITargetingFilterAccessor targetingAccessor = routes.ServiceProvider.GetService<ITargetingFilterAccessor>();
            IOnScreenContextProvider onScreenContextProvider = routes.ServiceProvider.GetService<IOnScreenContextProvider>();

            routes.Routes.Add(new ContentRoute(controllerMapper, targetingAccessor, routes.DefaultHandler, name, template,
                    new RouteValueDictionary(defaults),
                    new RouteValueDictionary(constraints),
                    new RouteValueDictionary(dataTokens),
                    requiredService,
                    onScreenContextProvider));

            return routes;
        }
    }
}
