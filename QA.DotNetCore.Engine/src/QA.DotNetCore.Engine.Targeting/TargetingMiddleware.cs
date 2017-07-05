using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Targeting
{
    public class TargetingMiddleware
    {
        readonly ITargetingProvidersConfigurator _targetingConfigurationBuilder;
        readonly RequestDelegate _next;

        public TargetingMiddleware(RequestDelegate next, ITargetingProvidersConfigurator p)
        {
            _next = next;
            _targetingConfigurationBuilder = p;
        }

        public Task Invoke(HttpContext context)
        {
            foreach (var provider in _targetingConfigurationBuilder.GetProviders())
            {
                var dict = provider.GetValues();
                foreach (var key in dict.Keys)
                {
                    context.Items[key] = dict[key];
                }
            }

            // Call the next delegate/middleware in the pipeline
            return _next(context);
        }
    }
}
