using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Routing.Exceptions;
using System;
using System.Threading;
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
            CancellationToken cancellationToken = context?.RequestAborted ?? CancellationToken.None;

            AbstractItemStorage abstractItems = null;
#if DEBUG
            int repeatCount = 0;
#endif

            while (abstractItems is null)
            {
#if DEBUG
                repeatCount++;
                if (repeatCount > 12)
                    throw new TooManyAttemptsToBuildSiteStructureException();
#endif
                abstractItems = provider.Get();

                if (abstractItems is null)
                    Thread.Sleep(TimeSpan.FromSeconds(5));
            }

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
