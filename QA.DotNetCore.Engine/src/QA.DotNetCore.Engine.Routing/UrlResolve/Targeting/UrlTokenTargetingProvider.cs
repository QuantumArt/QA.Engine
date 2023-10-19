using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching
{
    public class UrlTokenTargetingProvider : ITargetingProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHeadUrlResolver _urlResolver;
        public TargetingSource Source => TargetingSource.Primary;

        public UrlTokenTargetingProvider(IHttpContextAccessor httpContextAccessor, IHeadUrlResolver urlResolver)
        {
            _httpContextAccessor = httpContextAccessor;
            _urlResolver = urlResolver;
        }

        public IDictionary<string, object> GetValues()
        {
            return _urlResolver.ResolveTokenValues(GetAbsoluteUrl(_httpContextAccessor.HttpContext))
                    .ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value);
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
