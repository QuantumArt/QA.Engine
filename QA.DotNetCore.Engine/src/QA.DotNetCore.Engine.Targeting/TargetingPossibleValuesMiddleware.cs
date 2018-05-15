using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Targeting
{
    /// <summary>
    /// Middleware, которая сохраняет в HttpContext возможные значения таргетирования
    /// </summary>
    public class TargetingPossibleValuesMiddleware
    {
        readonly ServiceSetConfigurator<ITargetingPossibleValuesProvider> _targetingConfigurationBuilder;
        readonly RequestDelegate _next;

        public const string HttpContextKeyPrefix = "PossibleValues::";

        public TargetingPossibleValuesMiddleware(RequestDelegate next, ServiceSetConfigurator<ITargetingPossibleValuesProvider> p)
        {
            _next = next;
            _targetingConfigurationBuilder = p;
        }

        public Task Invoke(HttpContext context)
        {
            //сохраним в HttpContext возможные значения таргетирования по всем ключам
            foreach (var provider in _targetingConfigurationBuilder.GetServices())
            {
                var dict = provider.GetPossibleValues();
                foreach (var key in dict.Keys)
                {
                    context.Items[HttpContextKeyPrefix + key] = dict[key].ToArray();
                }
            }

            // Call the next delegate/middleware in the pipeline
            return _next(context);
        }
    }
}
