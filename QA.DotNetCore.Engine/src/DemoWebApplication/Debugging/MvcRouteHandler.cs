//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc.Core;
//using Microsoft.AspNetCore.Mvc.Infrastructure;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.Extensions.Logging;

//namespace DemoWebApplication.Debugging
//{
//    public class MvcRouteHandler : IRouter
//    {
//        private readonly IActionContextAccessor _actionContextAccessor;
//        private readonly IActionInvokerFactory _actionInvokerFactory;
//        private readonly IActionSelector _actionSelector;
//        private readonly ILogger _logger;

//        public MvcRouteHandler(
//            IActionInvokerFactory actionInvokerFactory,
//            IActionSelector actionSelector,
//            ILoggerFactory loggerFactory,
//            IActionContextAccessor actionContextAccessor)
//        {
//            // The IActionContextAccessor is optional. We want to avoid the overhead of using CallContext
//            // if possible.
//            _actionContextAccessor = actionContextAccessor;

//            _actionInvokerFactory = actionInvokerFactory;
//            _actionSelector = actionSelector;
//            _logger = loggerFactory.CreateLogger<MvcRouteHandler>();
//        }

//        public VirtualPathData GetVirtualPath(VirtualPathContext context)
//        {
//            if (context == null)
//            {
//                throw new ArgumentNullException(nameof(context));
//            }

//            // We return null here because we're not responsible for generating the url, the route is.
//            return null;
//        }

//        public Task RouteAsync(RouteContext context)
//        {
//            if (context == null)
//            {
//                throw new ArgumentNullException(nameof(context));
//            }

//            var candidates = _actionSelector.SelectCandidates(context);
//            if (candidates == null || candidates.Count == 0)
//            {
//                return Task.CompletedTask;
//            }

//            var actionDescriptor = _actionSelector.SelectBestCandidate(context, candidates);
//            if (actionDescriptor == null)
//            {
//                return Task.CompletedTask;
//            }

//            context.Handler = (c) =>
//            {
//                var routeData = c.GetRouteData();

//                var actionContext = new ActionContext(context.HttpContext, routeData, actionDescriptor);
//                if (_actionContextAccessor != null)
//                {
//                    _actionContextAccessor.ActionContext = actionContext;
//                }

//                var invoker = _actionInvokerFactory.CreateInvoker(actionContext);
//                if (invoker == null)
//                {
//                    throw new InvalidOperationException(
//                            actionDescriptor.DisplayName);
//                }

//                return invoker.InvokeAsync();
//            };

//            return Task.CompletedTask;
//        }
//    }
//}
