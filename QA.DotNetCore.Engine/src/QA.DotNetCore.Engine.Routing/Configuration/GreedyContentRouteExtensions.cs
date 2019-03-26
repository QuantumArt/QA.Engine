using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Routing.Configuration
{
    public static class GreedyContentRouteExtensions
    {
        private const string tailRouteTokenName = "tail";

        public static string TailRouteTokenName => tailRouteTokenName;

        public static IRouteBuilder MapGreedyContentRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string templatePrefix,
            object defaults,
            object constraints = null,
            object dataTokens = null)
        {
            IInlineConstraintResolver requiredService = routeBuilder.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();
            IControllerMapper controllerMapper = routeBuilder.ServiceProvider.GetRequiredService<IControllerMapper>();
            ITargetingFilterAccessor targetingAccessor = routeBuilder.ServiceProvider.GetService<ITargetingFilterAccessor>();
            IOnScreenContextProvider onScreenContextProvider = routeBuilder.ServiceProvider.GetService<IOnScreenContextProvider>();

            var template = CreateRouteTemplate(templatePrefix);
            var constraintsDict = ObjectToDictionary(constraints);
            constraintsDict.Add(TailRouteTokenName, new GreedyRouteConstraint(TailRouteTokenName));

            var route = new GreedyContentRoute(controllerMapper, targetingAccessor, routeBuilder.DefaultHandler, name, template,
                    new RouteValueDictionary(defaults),
                    constraintsDict,
                    new RouteValueDictionary(dataTokens),
                    requiredService,
                    onScreenContextProvider);

            routeBuilder.Routes.Add(route);

            return routeBuilder;
        }

        private static string CreateRouteTemplate(string templatePrefix)
        {
            templatePrefix = templatePrefix ?? string.Empty;

            if (templatePrefix.Contains("?"))
            {
                // TODO: Consider supporting this. The {*clientRoute} part should be added immediately before the '?'
                throw new ArgumentException("Route templates don't support querystrings");
            }

            if (templatePrefix.Contains("#"))
            {
                throw new ArgumentException(
                    "Route templates should not include # characters. The hash part of a URI does not get sent to the server.");
            }

            if (templatePrefix != string.Empty && !templatePrefix.EndsWith("/"))
            {
                templatePrefix += "/";
            }

            return templatePrefix + $"{{*{TailRouteTokenName}}}";
        }

        private static IDictionary<string, object> ObjectToDictionary(object value)
            => value as IDictionary<string, object> ?? new RouteValueDictionary(value);
    }
}
