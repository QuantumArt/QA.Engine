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

        public WidgetViewComponentInvoker(IViewComponentInvoker innerInvoker)
        {
            _inner = innerInvoker;
        }

        public Task InvokeAsync(ViewComponentContext context)
        {
            var currentItem= context.ViewContext.HttpContext.Items["ui-part"] as AbstractItem;
            context.Arguments["currentItem"] = currentItem;
            context.ViewData["CurrentItem"] = currentItem;
            return _inner.InvokeAsync(context);
        }
    }
}
