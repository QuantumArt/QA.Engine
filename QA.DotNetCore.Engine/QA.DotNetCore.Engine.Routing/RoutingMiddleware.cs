using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Routing
{
    public class RoutingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAbstractItemStorageProvider _provider;

        public RoutingMiddleware(RequestDelegate next, IAbstractItemStorageProvider p)
        {
            _next = next;
            _provider = p;
        }

        public Task Invoke(HttpContext context)
        {
            var startPage = _provider
                .Get()
                .GetStartPage(context.Request.Host.Value);

            context.Items["start-page"] = startPage;

            // Call the next delegate/middleware in the pipeline
            return _next(context);
        }
    }
}
