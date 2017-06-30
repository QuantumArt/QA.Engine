using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Routing
{
    public class RoutingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAbstractItemStorageProvider _provider;
        private readonly ITargetingFilterAccessor _filterAccessor;

        public RoutingMiddleware(RequestDelegate next, IAbstractItemStorageProvider p, ITargetingFilterAccessor filterAccessor = null)
        {
            _next = next;
            _provider = p;
            _filterAccessor = filterAccessor;
        }

        public Task Invoke(HttpContext context)
        {
            var startPage = _provider
                .Get()
                .GetStartPage(context.Request.Host.Value, _filterAccessor?.Get());

            context.Items["start-page"] = startPage;

            // Call the next delegate/middleware in the pipeline
            return _next(context);
        }
    }
}
