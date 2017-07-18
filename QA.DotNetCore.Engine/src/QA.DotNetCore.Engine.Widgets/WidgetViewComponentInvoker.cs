using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using QA.DotNetCore.Engine.Abstractions;

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
            var currentItem = context.ViewContext.HttpContext.Items["ui-part"] as IAbstractItem;

            context.Arguments["currentItem"] = currentItem ?? throw new InvalidOperationException("Current item is null");

            context.ViewData["CurrentItem"] = currentItem;

            return _inner.InvokeAsync(context);

        }
    }
}
