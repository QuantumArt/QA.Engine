using System;
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Widgets
{
    public class WidgetViewComponentInvokerFactory : IViewComponentInvokerFactory
    {
        private readonly IViewComponentFactory _viewComponentFactory;

        private readonly ViewComponentInvokerCache _viewComponentInvokerCache;

        private readonly ILogger _logger;

        private readonly DiagnosticSource _diagnosticSource;
        private readonly WidgetContextScope _scope;

        public WidgetViewComponentInvokerFactory(IViewComponentFactory viewComponentFactory, ViewComponentInvokerCache viewComponentInvokerCache, DiagnosticSource diagnosticSource, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException("loggerFactory");
            }
            this._viewComponentFactory = viewComponentFactory ?? throw new ArgumentNullException("viewComponentFactory");
            this._diagnosticSource = diagnosticSource ?? throw new ArgumentNullException("diagnosticSource");
            this._viewComponentInvokerCache = viewComponentInvokerCache ?? throw new ArgumentNullException("viewComponentInvokerCache");
            this._logger = loggerFactory.CreateLogger<DefaultViewComponentInvoker>();
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
                return GetDefaultInvoker();
            }
            //сейчас рендерится компонент как виджет структуры сайта
            //это ясно по одноразовому флагу ShouldUseCustomInvoker, снимем его
            renderingContext.ShouldUseCustomInvoker = false;

            return new WidgetViewComponentInvoker(_scope, GetDefaultInvoker());
        }

        private DefaultViewComponentInvoker GetDefaultInvoker()
        {
            return new DefaultViewComponentInvoker(this._viewComponentFactory,
                            this._viewComponentInvokerCache,
                            this._diagnosticSource,
                            this._logger);
        }
    }
}
