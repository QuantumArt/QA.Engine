using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching
{
    public class UrlTokenTargetingProvider : ITargetingProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlTokenTargetingProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IDictionary<string, object> GetValues()
        {
            var urlResolver = _httpContextAccessor.HttpContext.GetStartPage()?.GetUrlResolver();
            if (urlResolver != null)
            { 
                return urlResolver.ResolveTargetingValuesFromUrl(GetAbsoluteUrl(_httpContextAccessor.HttpContext))
                    .ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value);
            }
            return new Dictionary<string, object>(0);
        }

        private string GetAbsoluteUrl(HttpContext context)
        {
            var request = context.Request;
            UriBuilder uriBuilder = new UriBuilder
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Path = request.Path.ToString(),
                Query = request.QueryString.ToString()
            };
            return uriBuilder.Uri.ToString();
        }
    }
}
