using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Internal;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Routing
{
    public class ContentRoute : Route
    {
        private ILogger _constraintLogger;
        private ILogger _logger;
        private TemplateMatcher _matcher;
        private TemplateBinder _binder;
        private readonly IControllerMapper _mapper;
        private readonly ITargetingFilterAccessor _targetingProvider;

        public ContentRoute(IControllerMapper controllerMapper, ITargetingFilterAccessor targetingProvider, IRouter target, string routeTemplate, IInlineConstraintResolver inlineConstraintResolver)
            : this(controllerMapper, targetingProvider, target, routeTemplate, null, null, null, inlineConstraintResolver)
        {
        }

        public ContentRoute(IControllerMapper controllerMapper, ITargetingFilterAccessor targetingProvider, IRouter target, string routeTemplate, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver)
            : this(controllerMapper, targetingProvider, target, null, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
        }

        public ContentRoute(IControllerMapper controllerMapper, ITargetingFilterAccessor targetingProvider, IRouter target, string routeName, string routeTemplate, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver)
            : base(target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
            _mapper = controllerMapper;
            _targetingProvider = targetingProvider;
        }


        public override Task RouteAsync(RouteContext context)
        {
            PathString path = context.HttpContext.Request.Path;

            EnsureMatcher();

            EnsureLoggers(context.HttpContext);

            var startPage = context.HttpContext.Items["start-page"] as IAbstractItem;//проставляется в RoutingMiddleware
            if (startPage == null)
            {
                return Task.FromResult<int>(0);
            }

            var targetingFilter = _targetingProvider.Get();
            var data = (context.HttpContext.Items["current-page"] as PathData) ?? new PathFinder().Find(path, startPage, targetingFilter);

            if (data != null)
            {
                path = data.RemainingUrl;
                context.HttpContext.Items["current-page"] = data;
            }

            var controllerName = _mapper.Map(data.AbstractItem);

            if (string.IsNullOrEmpty(controllerName))
            {
                return Task.FromResult<int>(0);
            }

            path = $"/{controllerName}{path.Value}";

            if (!this._matcher.TryMatch(path, context.RouteData.Values))
            {
                return Task.FromResult<int>(0);
            }

            if (data != null)
            {
                context.RouteData.Values["ui-item"] = data.AbstractItem.Id;
                context.RouteData.DataTokens["ui-item"] = data.AbstractItem;
            }

            if (this.DataTokens.Count > 0)
            {
                //RouteBase.MergeValues(context.RouteData.DataTokens, this.DataTokens);
                foreach (KeyValuePair<string, object> current in this.DataTokens)
                {
                    context.RouteData.DataTokens[current.Key] = current.Value;
                }
            }

            if (!RouteConstraintMatcher.Match(this.Constraints,
                context.RouteData.Values,
                context.HttpContext, this,
                RouteDirection.IncomingRequest,
                this._constraintLogger))
            {
                return Task.FromResult<int>(0);
            }

            return this.OnRouteMatched(context);
        }


        public override VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            this.EnsureBinder(context.HttpContext);
            this.EnsureLoggers(context.HttpContext);

            if (!context.HttpContext.GetRouteData().DataTokens.ContainsKey("ui-item") &&
                 !context.HttpContext.Items.ContainsKey("ui-part"))
            {
                return null;
            }

            // TODO: надо бы проверить, что контроллер соответствует запрашиваемому типу

            TemplateValuesResult values = this._binder.GetValues(context.AmbientValues, context.Values);
            if (values == null)
            {
                return null;
            }
            if (!RouteConstraintMatcher.Match(this.Constraints,
                values.CombinedValues,
                context.HttpContext, this,
                RouteDirection.UrlGeneration,
                this._constraintLogger))
            {
                return null;
            }
            context.Values = values.CombinedValues;
            VirtualPathData virtualPathData = this.OnVirtualPathGenerated(context);
            if (virtualPathData != null)
            {
                return virtualPathData;
            }


            var part = (context.HttpContext.Items["ui-part"] as IAbstractItem);
            var page = (context.HttpContext.GetRouteData().DataTokens["ui-item"] as IAbstractItem);

            if (page == null && part == null)
            {
                return null;
            }


            var controllerName = _mapper.Map(page);

            if (!string.Equals(controllerName, context.Values["controller"]))
            {
                return null;
            }

            if (page != null && part == null)
            {
                values.AcceptedValues["controller"] = "--replace-with-path--";

                values.AcceptedValues.Remove("ui-item");
            }
            else if(part != null)
            {                
                values.AcceptedValues["ui-part"] = part.Id;
            }

            string text = this._binder.BindValues(values.AcceptedValues);
            if (text == null)
            {
                return null;
            }

            if (page != null)
            {
                text = text.Replace("--replace-with-path--", page.GetTrail().TrimStart('/'));
            }
            virtualPathData = new VirtualPathData(this, text);

            if (this.DataTokens != null)
            {
                foreach (KeyValuePair<string, object> current in this.DataTokens)
                {
                    virtualPathData.DataTokens.Add(current.Key, current.Value);
                }
            }

            return virtualPathData;
        }


        private void EnsureMatcher()
        {
            if (this._matcher == null)
            {
                this._matcher = new TemplateMatcher(this.ParsedTemplate, this.Defaults);
            }
        }

        private void EnsureLoggers(HttpContext context)
        {
            if (this._logger == null)
            {
                ILoggerFactory requiredService = context.RequestServices.GetRequiredService<ILoggerFactory>();
                this._logger = requiredService.CreateLogger(typeof(RouteBase).FullName);
                this._constraintLogger = requiredService.CreateLogger(typeof(RouteConstraintMatcher).FullName);
            }
        }

        private void EnsureBinder(HttpContext context)
        {
            if (this._binder == null)
            {
                UrlEncoder requiredService = context.RequestServices.GetRequiredService<UrlEncoder>();
                ObjectPool<UriBuildingContext> requiredService2 = context.RequestServices.GetRequiredService<ObjectPool<UriBuildingContext>>();
                this._binder = new TemplateBinder(requiredService, requiredService2, this.ParsedTemplate, this.Defaults);
            }
        }

        protected override async Task OnRouteMatched(RouteContext context)
        {
            await base.OnRouteMatched(context);
        }
    }
}
