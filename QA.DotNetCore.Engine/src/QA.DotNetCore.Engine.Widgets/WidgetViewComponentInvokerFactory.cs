using System;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace QA.DotNetCore.Engine.Widgets
{
    public class WidgetViewComponentInvokerFactory : IViewComponentInvokerFactory
    {
        private readonly IViewComponentInvokerFactory _defaultFactory;
        private readonly WidgetContextScope _scope;

        public WidgetViewComponentInvokerFactory(IViewComponentInvokerFactory defaultFactory)
        {
            _defaultFactory = defaultFactory;
            this._scope = new WidgetContextScope();
        }

        public IViewComponentInvoker CreateInstance(ViewComponentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var renderingContext = context.ViewContext.HttpContext.GetCurrentRenderingWidgetContext();
            if (renderingContext == null || !renderingContext.ShouldUseCustomInvoker)
            {
                //если стэка виджетов нет, или в нём нет отметки, что сейчас рендерится компонент как виджет структуры сайта
                //используем стандартный Invoker для компонентов
                return GetDefaultInvoker(context);
            }
            //сейчас рендерится компонент как виджет структуры сайта
            //это ясно по одноразовому флагу ShouldUseCustomInvoker, снимем его
            renderingContext.ShouldUseCustomInvoker = false;

            return new WidgetViewComponentInvoker(_scope, GetDefaultInvoker(context));
        }

        private IViewComponentInvoker GetDefaultInvoker(ViewComponentContext context)
        {
            return _defaultFactory.CreateInstance(context);
        }
    }
}
