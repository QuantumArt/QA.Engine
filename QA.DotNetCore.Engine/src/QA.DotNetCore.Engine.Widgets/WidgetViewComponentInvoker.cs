using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using QA.DotNetCore.Engine.Abstractions;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Widgets
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
            IAbstractItem currentItem = context.ViewContext.HttpContext.GetCurrentRenderingWidgetContext()?.CurrentWidget;

            context.Arguments["currentItem"] = currentItem ?? throw new InvalidOperationException("Current item is null");

            return _inner.InvokeAsync(context);

        }
    }
}
