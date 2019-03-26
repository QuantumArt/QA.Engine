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
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Routing
{
    public abstract class AbstractContentRoute : Route
    {
        protected ILogger ConstraintLogger;
        protected ILogger Logger;
        protected TemplateMatcher Matcher;
        protected TemplateBinder Binder;
        protected readonly IControllerMapper ControllerMapper;
        protected readonly ITargetingFilterAccessor TargetingProvider;

        public AbstractContentRoute(IControllerMapper controllerMapper, ITargetingFilterAccessor targetingProvider, IRouter target, string routeTemplate, IInlineConstraintResolver inlineConstraintResolver)
            : this(controllerMapper, targetingProvider, target, routeTemplate, null, null, null, inlineConstraintResolver)
        {
        }

        public AbstractContentRoute(IControllerMapper controllerMapper, ITargetingFilterAccessor targetingProvider, IRouter target, string routeTemplate, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver)
            : this(controllerMapper, targetingProvider, target, null, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
        }

        public AbstractContentRoute(IControllerMapper controllerMapper, ITargetingFilterAccessor targetingProvider, IRouter target, string routeName, string routeTemplate, RouteValueDictionary defaults, IDictionary<string, object> constraints, RouteValueDictionary dataTokens, IInlineConstraintResolver inlineConstraintResolver)
            : base(target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
            ControllerMapper = controllerMapper;
            TargetingProvider = targetingProvider;
        }

        protected abstract PathFinder CreatePathFinder();

        protected abstract bool SavePathDataInHttpContext { get; }

        public override Task RouteAsync(RouteContext context)
        {
            PathString path = context.HttpContext.Request.Path;

            EnsureMatcher();
            
            EnsureLoggers(context.HttpContext);

            var startPage = context.HttpContext.Items[RoutingKeys.StartPage] as IStartPage;//проставляется в RoutingMiddleware
            if (startPage == null)
            {
                return Task.FromResult<int>(0);
            }

            var targetingFilter = TargetingProvider?.Get();

            PathData data = null;

            if (SavePathDataInHttpContext)
                data = context.HttpContext.Items["current-page"] as PathData;

            var onScreenContext = context.HttpContext.Items[OnScreenModeKeys.OnScreenContext] as OnScreenContext;
            var abstractItemStorage = context.HttpContext.Items[RoutingKeys.AbstractItemStorage] as AbstractItemStorage;
            if (onScreenContext?.Enabled == true && onScreenContext?.PageId.HasValue == true && abstractItemStorage != null)
            {
                var abstractItem = abstractItemStorage.Get(onScreenContext.PageId.Value);
                var isContainInStartPage = IsStartPageContainAbstractItem(startPage, abstractItem);
                if (isContainInStartPage)
                    data = new PathData(abstractItem, "");
            }

            if (data == null)
                data = CreatePathFinder().Find(path, startPage, targetingFilter);

            if (data != null)
            {
                path = data.RemainingUrl;

                if (SavePathDataInHttpContext)
                    context.HttpContext.Items["current-page"] = data;
            }

            var controllerName = ControllerMapper.Map(data.AbstractItem);

            if (string.IsNullOrEmpty(controllerName))
            {
                return Task.FromResult<int>(0);
            }

            path = $"/{controllerName}{path.Value}";

            if (!this.Matcher.TryMatch(path, context.RouteData.Values))
            {
                return Task.FromResult<int>(0);
            }

            if (data != null)
            {
                context.RouteData.Values[RoutingKeys.CurrentItem] = data.AbstractItem.Id;
                context.RouteData.DataTokens[RoutingKeys.CurrentItem] = data.AbstractItem;

                var parsedControllerName = (string)context.RouteData.Values["controller"];
                if (!string.Equals(parsedControllerName, controllerName, System.StringComparison.OrdinalIgnoreCase))
                {
                    // проверяем, что полученный из роута контроллер соответствует контроллеру,
                    // полученному из типа страницы
                    return Task.FromResult<int>(0);
                }
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
                this.ConstraintLogger))
            {
                return Task.FromResult<int>(0);
            }

            return this.OnRouteMatched(context);
        }


        public override VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            this.EnsureBinder(context.HttpContext);
            this.EnsureLoggers(context.HttpContext);

            if (!context.HttpContext.GetRouteData().DataTokens.ContainsKey(RoutingKeys.CurrentItem) &&
                 !context.HttpContext.Items.ContainsKey(RoutingKeys.CurrentWidget))
            {
                return null;
            }

            // TODO: надо бы проверить, что контроллер соответствует запрашиваемому типу

            TemplateValuesResult values = this.Binder.GetValues(context.AmbientValues, context.Values);
            if (values == null)
            {
                return null;
            }
            if (!RouteConstraintMatcher.Match(this.Constraints,
                values.CombinedValues,
                context.HttpContext, this,
                RouteDirection.UrlGeneration,
                this.ConstraintLogger))
            {
                return null;
            }
            context.Values = values.CombinedValues;
            VirtualPathData virtualPathData = this.OnVirtualPathGenerated(context);
            if (virtualPathData != null)
            {
                return virtualPathData;
            }


            var part = (context.HttpContext.Items[RoutingKeys.CurrentWidget] as IAbstractItem);
            var page = (context.HttpContext.GetRouteData().DataTokens[RoutingKeys.CurrentItem] as IAbstractItem);

            if (page == null && part == null)
            {
                return null;
            }


            var controllerName = ControllerMapper.Map(page);

            if (!string.Equals(controllerName, context.Values["controller"]))
            {
                return null;
            }

            if (page != null && part == null)
            {
                values.AcceptedValues["controller"] = "--replace-with-path--";

                values.AcceptedValues.Remove(RoutingKeys.CurrentItem);
            }
            else if (part != null)
            {
                values.AcceptedValues[RoutingKeys.CurrentWidget] = part.Id;
            }

            string text = this.Binder.BindValues(values.AcceptedValues);
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


        protected virtual void EnsureMatcher()
        {
            if (this.Matcher == null)
            {
                this.Matcher = new TemplateMatcher(this.ParsedTemplate, this.Defaults);
            }
        }

        protected virtual void EnsureLoggers(HttpContext context)
        {
            if (this.Logger == null)
            {
                ILoggerFactory requiredService = context.RequestServices.GetRequiredService<ILoggerFactory>();
                this.Logger = requiredService.CreateLogger(typeof(RouteBase).FullName);
                this.ConstraintLogger = requiredService.CreateLogger(typeof(RouteConstraintMatcher).FullName);
            }
        }

        protected virtual void EnsureBinder(HttpContext context)
        {
            if (this.Binder == null)
            {
                UrlEncoder requiredService = context.RequestServices.GetRequiredService<UrlEncoder>();
                ObjectPool<UriBuildingContext> requiredService2 = context.RequestServices.GetRequiredService<ObjectPool<UriBuildingContext>>();
                this.Binder = new TemplateBinder(requiredService, requiredService2, this.ParsedTemplate, this.Defaults);
            }
        }

        protected override async Task OnRouteMatched(RouteContext context)
        {
            await base.OnRouteMatched(context);
        }

        private bool IsStartPageContainAbstractItem(IStartPage startPage, IAbstractItem abstractItem)
        {
            if (startPage == null || abstractItem == null)
                return false;
            while (abstractItem.Parent != null)
            {
                if (abstractItem.Id == startPage.Id)
                    return true;
                abstractItem = abstractItem.Parent;
            }
            return false;
        }
    }
}
