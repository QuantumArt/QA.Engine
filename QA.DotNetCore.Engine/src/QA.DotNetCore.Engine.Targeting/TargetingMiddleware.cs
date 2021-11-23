using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Targeting
{
    /// <summary>
    /// Middleware, которая сохраняет в HttpContext текущие значения таргетирования
    /// </summary>
    public class TargetingMiddleware
    {
        readonly ServiceSetConfigurator<ITargetingProvider> _targetingConfigurationBuilder;
        readonly ServiceSetConfigurator<ITargetingProviderAsync> _targetingConfigurationBuilderForAsync;
        readonly RequestDelegate _next;

        public TargetingMiddleware(RequestDelegate next,
            ServiceSetConfigurator<ITargetingProvider> p,
            ServiceSetConfigurator<ITargetingProviderAsync> p2)
        {
            _next = next;
            _targetingConfigurationBuilder = p;
            _targetingConfigurationBuilderForAsync = p2;
        }

        public async Task Invoke(HttpContext context)
        {
            //сохраним в HttpContext все значения таргетирования, как из синхронных провайдеров, так и асинхронных
            var targetingKeys = new List<string>();

            var getValuesTasks = _targetingConfigurationBuilderForAsync.GetServices(context.RequestServices).Select(tp => tp.GetValues());
            await Task.WhenAll(getValuesTasks);

            foreach (var dict in getValuesTasks
                .Select(t => t.Result)
                .Union(_targetingConfigurationBuilder.GetServices(context.RequestServices).Select(tp => tp.GetValues()))
                .Where(dict => dict != null))
            {
                foreach (var key in dict.Keys)
                {
                    context.Items[key] = dict[key];
                    targetingKeys.Add(key);
                }
            }

            //сохраним в HttpContext все полученные ключи таргетирования
            context.Items[HttpTargetingContext.TargetingKeysContextKey] = targetingKeys.Distinct().ToArray();

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
