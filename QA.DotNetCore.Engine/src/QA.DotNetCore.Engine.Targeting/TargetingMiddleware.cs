using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;
using System.Linq;
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
            //сохраним в HttpContext все значения таргетирования
            var targetingKeys = new List<string>();
            foreach (var provider in _targetingConfigurationBuilder.GetProviders())
            {
                var dict = provider.GetValues();
                foreach (var key in dict.Keys)
                {
                    context.Items[key] = dict[key];
                    targetingKeys.Add(key);
                }
            }

            //сохраним в HttpContext все полученные ключи таргетирования
            context.Items[HttpTargetingContext.TargetingKeysContextKey] = targetingKeys.Distinct().ToArray();

            // Call the next delegate/middleware in the pipeline
            return _next(context);
        }
    }
}
