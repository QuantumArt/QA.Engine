// Got from https://github.com/aspnet/JavaScriptServices/blob/dev/src/Microsoft.AspNetCore.SpaServices/Routing/SpaRouteConstraint.cs
// as the SpaRouteConstraint is internal

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;


namespace QA.DotNetCore.Engine.Routing.Configuration
{
    /// <summary>
    /// Checks that given token does not contain dots.
    /// </summary>
    public class GreedyRouteConstraint : IRouteConstraint
    {        
        private readonly string _tailRouteTokenName;

        public GreedyRouteConstraint(string clientRouteTokenName)
        {
            if (string.IsNullOrEmpty(clientRouteTokenName))
            {
                throw new ArgumentException("Value cannot be null or empty", nameof(clientRouteTokenName));
            }

            _tailRouteTokenName = clientRouteTokenName;
        }

        public bool Match(
            HttpContext httpContext,
            IRouter route,
            string routeKey,
            RouteValueDictionary values,
            RouteDirection routeDirection)
        {
            return !HasDotInLastSegment(values[_tailRouteTokenName] as string ?? string.Empty);
        }

        private bool HasDotInLastSegment(string uri)
        {
            var lastSegmentStartPos = uri.LastIndexOf('/');
            return uri.IndexOf('.', lastSegmentStartPos + 1) >= 0;
        }
    }
}
