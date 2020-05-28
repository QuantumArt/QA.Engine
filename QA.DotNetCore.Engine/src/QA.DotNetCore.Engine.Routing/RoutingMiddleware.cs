using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Routing.Exceptions;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Routing
{
    public class RoutingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITargetingFilterAccessor _filterAccessor;

        public RoutingMiddleware(RequestDelegate next, ITargetingFilterAccessor filterAccessor = null)
        {
            _next = next;
            _filterAccessor = filterAccessor;
        }

        public Task Invoke(HttpContext context, IAbstractItemStorageProvider provider)
        {
            var abstractItems = provider
                .Get();
            var startPage = abstractItems
                .GetStartPage(context.Request.Host.Value, _filterAccessor?.Get());

            if (startPage is null)
                throw new StartPageNotFoundException(context.Request.Host.Value);

            context.Items[RoutingKeys.StartPage] = startPage;
            context.Items[RoutingKeys.AbstractItemStorage] = abstractItems;

            // Call the next delegate/middleware in the pipeline
            return _next(context);
        }
    }
}
