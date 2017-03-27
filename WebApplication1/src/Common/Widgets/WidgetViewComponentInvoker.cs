using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Common.PageModel;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Logging;

namespace Common.Widgets
{
    public class WidgetViewComponentInvoker : IViewComponentInvoker
    {
        private readonly IViewComponentInvoker _inner;
        private readonly WidgetContextScope _scope;

        public WidgetViewComponentInvoker(WidgetContextScope scope, IViewComponentInvoker innerInvoker)
        {
            _inner = innerInvoker;
            _scope = scope;
        }

        public Task InvokeAsync(ViewComponentContext context)
        {
            var currentItem = context.ViewContext.HttpContext.Items["ui-part"] as AbstractItem;

            if(currentItem == null)
            {
                throw new InvalidOperationException("Current item is null");
            }

            context.Arguments["currentItem"] = currentItem;
            context.ViewData["CurrentItem"] = currentItem;

            return _inner.InvokeAsync(context);

        }
    }
}
