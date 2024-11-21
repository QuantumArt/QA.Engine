using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Routing.Exceptions;
using System.Threading;
using System.Threading.Tasks;
using QA.DotNetCore.Engine.Routing.Configuration;

namespace QA.DotNetCore.Engine.Routing
{
    public class RoutingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITargetingFilterAccessor _filterAccessor;
        private readonly ExcludePathChecker _checker;

        public RoutingMiddleware(RequestDelegate next, ExcludePathChecker checker, ITargetingFilterAccessor filterAccessor = null)
        {
            _next = next;
            _checker = checker;
            _filterAccessor = filterAccessor ?? new NullTargetingFilterAccessor();
        }

        public Task Invoke(HttpContext context, IAbstractItemStorageProvider provider)
        {
            if (_checker != null && _checker.IsExcluded(context.Request.Path))
            {
                return _next(context);
            }

            CancellationToken cancellationToken = context?.RequestAborted ?? CancellationToken.None;

            AbstractItemStorage abstractItems = provider.Get();

            if (cancellationToken.IsCancellationRequested)
                return Task.CompletedTask;

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
