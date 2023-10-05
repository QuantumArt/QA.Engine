using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Targeting
{
    /// <summary>
    /// Middleware, которая сохраняет в HttpContext текущие значения таргетирования
    /// </summary>
    public class TargetingMiddleware
    {

        ITargetingContextUpdater _updater;
        readonly RequestDelegate _next;

        public TargetingMiddleware(RequestDelegate next, ITargetingContextUpdater updater)
        {
            _next = next;
            _updater = updater;
        }

        public async Task Invoke(HttpContext context)
        {
            await _updater.Update(null);

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
