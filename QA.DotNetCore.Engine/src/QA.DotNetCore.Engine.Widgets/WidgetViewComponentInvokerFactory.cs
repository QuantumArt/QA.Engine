using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Logging;

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

            if (context.ViewContext.HttpContext.Items["should-use-custom-invoker"] == null)
            {
                return GetDefaultInvoker();
            }

            // remove flag to use defualt invoker for non widget components
            context.ViewContext.HttpContext.Items.Remove("should-use-custom-invoker");

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
