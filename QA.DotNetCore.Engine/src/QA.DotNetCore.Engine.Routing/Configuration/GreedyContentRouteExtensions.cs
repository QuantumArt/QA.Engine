// Based on https://github.com/aspnet/JavaScriptServices/blob/dev/src/Microsoft.AspNetCore.SpaServices/Routing/SpaRouteExtensions.cs
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Routing.Configuration
{
    /// <summary>
    /// Extension methods useful for configuring routing in a single-page application (SPA).
    /// </summary>
    public static class GreedyContentRouteExtensions
    {
        private const string tailRouteTokenName = "tail";

        public static string TailRouteTokenName => tailRouteTokenName;

        /// <summary>
        /// Configures a route that is automatically bypassed if the requested URL appears to be for a static file
        /// (e.g., if it has a filename extension).
        /// </summary>
        /// <param name="routeBuilder">The <see cref="IRouteBuilder"/>.</param>
        /// <param name="name">The route name.</param>
        /// <param name="templatePrefix">The template prefix.</param>
        /// <param name="defaults">Default route parameters.</param>
        /// <param name="constraints">Route constraints.</param>
        /// <param name="dataTokens">Route data tokens.</param>
        public static void MapGreedyContentRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string templatePrefix,
            object defaults,
            object constraints = null,
            object dataTokens = null)
        {
            var template = CreateRouteTemplate(templatePrefix);
            var constraintsDict = ObjectToDictionary(constraints);
            constraintsDict.Add(TailRouteTokenName, new GreedyRouteConstraint(TailRouteTokenName));

            routeBuilder.MapContentRoute(name, template, defaults, constraintsDict, dataTokens);
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
