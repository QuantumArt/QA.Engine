using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.Extensions.Logging;

namespace Common.Widgets
{
    public class WidgetViewComponentInvokerFactory : IViewComponentInvokerFactory
    {
        private readonly IViewComponentFactory _viewComponentFactory;

        private readonly ViewComponentInvokerCache _viewComponentInvokerCache;

        private readonly ILogger _logger;

        private readonly DiagnosticSource _diagnosticSource;

        public WidgetViewComponentInvokerFactory(IViewComponentFactory viewComponentFactory, ViewComponentInvokerCache viewComponentInvokerCache, DiagnosticSource diagnosticSource, ILoggerFactory loggerFactory)
        {
            if (viewComponentFactory == null)
            {
                throw new ArgumentNullException("viewComponentFactory");
            }
            if (viewComponentInvokerCache == null)
            {
                throw new ArgumentNullException("viewComponentInvokerCache");
            }
            if (diagnosticSource == null)
            {
                throw new ArgumentNullException("diagnosticSource");
            }
            if (loggerFactory == null)
            {
                throw new ArgumentNullException("loggerFactory");
            }
            this._viewComponentFactory = viewComponentFactory;
            this._diagnosticSource = diagnosticSource;
            this._viewComponentInvokerCache = viewComponentInvokerCache;
            this._logger = loggerFactory.CreateLogger<DefaultViewComponentInvoker>();
        }

        public IViewComponentInvoker CreateInstance(ViewComponentContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            return new WidgetViewComponentInvoker(new DefaultViewComponentInvoker(this._viewComponentFactory,
                this._viewComponentInvokerCache,
                this._diagnosticSource,
                this._logger));
        }
    }
}
