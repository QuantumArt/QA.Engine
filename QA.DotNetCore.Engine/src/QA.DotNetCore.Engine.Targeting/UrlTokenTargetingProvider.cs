using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Targeting
{
    public class UrlTokenTargetingProvider : ITargetingProvider
    {
        private readonly ITargetingUrlResolver _targetingUrlResolver;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlTokenTargetingProvider(ITargetingUrlResolver targetingUrlResolver,
            IHttpContextAccessor httpContextAccessor)
        {
            _targetingUrlResolver = targetingUrlResolver;
            _httpContextAccessor = httpContextAccessor;
        }

        public IDictionary<string, object> GetValues()
        {
            return _targetingUrlResolver.ResolveTargetingValuesFromUrl(GetAbsoluteUrl(_httpContextAccessor.HttpContext))
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
