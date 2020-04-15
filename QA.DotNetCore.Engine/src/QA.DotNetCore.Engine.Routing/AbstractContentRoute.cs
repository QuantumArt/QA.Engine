using Microsoft.AspNetCore.Routing;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Routing
{
    public abstract class AbstractContentRoute : Route
    {
        protected readonly IControllerMapper ControllerMapper;
        protected readonly ITargetingFilterAccessor TargetingProvider;

        public AbstractContentRoute(
            IControllerMapper controllerMapper,
            ITargetingFilterAccessor targetingProvider,
            IRouter target,
            string routeTemplate,
            IInlineConstraintResolver inlineConstraintResolver)
            : this(controllerMapper, targetingProvider, target, routeTemplate, null, null, null, inlineConstraintResolver)
        {
        }

        public AbstractContentRoute(
            IControllerMapper controllerMapper,
            ITargetingFilterAccessor targetingProvider,
            IRouter target,
            string routeTemplate,
            RouteValueDictionary defaults,
            IDictionary<string, object> constraints,
            RouteValueDictionary dataTokens,
            IInlineConstraintResolver inlineConstraintResolver)
            : this(controllerMapper, targetingProvider, target, null, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
        }

        public AbstractContentRoute(
            IControllerMapper controllerMapper,
            ITargetingFilterAccessor targetingProvider,
            IRouter target,
            string routeName,
            string routeTemplate,
            RouteValueDictionary defaults,
            IDictionary<string, object> constraints,
            RouteValueDictionary dataTokens,
            IInlineConstraintResolver inlineConstraintResolver)
            : base(target, routeName, routeTemplate, defaults, constraints, dataTokens, inlineConstraintResolver)
        {
            ControllerMapper = controllerMapper;
            TargetingProvider = targetingProvider;
        }

        protected abstract PathFinder CreatePathFinder();

        protected abstract bool SavePathDataInHttpContext { get; }

        public override Task RouteAsync(RouteContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var startPage = context.HttpContext.Items[RoutingKeys.StartPage] as IStartPage;//стартовая страница, проставляется в RoutingMiddleware
            if (startPage == null)
            {
                return Task.CompletedTask;
            }

            var targetingFilter = TargetingProvider?.Get();//все зарегистрированные фильтры структуры сайта, объединенные в один

            //вычислим PathData текущего запроса - какая страница в структуре сайта должна быть активирована
            PathData data = null;

            if (SavePathDataInHttpContext)
            {
                //может быть в рамках текущего запроса мы уже вычислили PathData и сохранили его в HttpContext
                //тогда получим его оттуда и не будем повторно вычислять
                data = context.HttpContext.Items[RoutingKeys.PathData] as PathData;
            }

            if (data == null)
            {
                //особый кейс: хотим открыть страницу сайта по её id, например, из админки структуры сайта
                //в этом случае мы находимся в режиме onScreen и передан id страницы (abstractItem'а)
                var onScreenContext = context.HttpContext.Items[OnScreenModeKeys.OnScreenContext] as OnScreenContext;
                var abstractItemStorage = context.HttpContext.Items[RoutingKeys.AbstractItemStorage] as AbstractItemStorage;
                
                if (onScreenContext?.Enabled == true && onScreenContext?.PageId.HasValue == true && abstractItemStorage != null)
                {
                    var abstractItem = abstractItemStorage.Get(onScreenContext.PageId.Value);
                    var isContainInStartPage = IsStartPageContainAbstractItem(startPage, abstractItem);
                    if (isContainInStartPage)
                        data = new PathData(abstractItem, "");
                }
            }

            var path = context.HttpContext.Request.Path;
            if (data == null)
            {
                //вычислим PathData разбирая урл запроса, и сопоставляя сегменты со структурой сайта
                data = CreatePathFinder().Find(path, startPage, targetingFilter);
            }

            if (data == null)
            {
                return Task.CompletedTask;
            }

            if (SavePathDataInHttpContext)
                context.HttpContext.Items[RoutingKeys.PathData] = data;

            var controllerName = ControllerMapper.Map(data.AbstractItem);
            if (string.IsNullOrEmpty(controllerName))
            {
                return Task.CompletedTask;
            }

            //проставим AbstractItem в RouteData
            context.RouteData.Values[RoutingKeys.CurrentItem] = data.AbstractItem.Id;
            context.RouteData.DataTokens[RoutingKeys.CurrentItem] = data.AbstractItem;

            //подменим HttpContext.Request.Path, т.к. он используется в RouteBase при матчинге с шаблоном роута
            context.HttpContext.Items["original-path"] = context.HttpContext.Request.Path;
            context.HttpContext.Request.Path = $"/{controllerName}{data.RemainingUrl}";

            //альтернативно можно вместо использования механизма c матчингом шаблона из RouteBase
            //самому наполнить context.RouteData.Values, но тогда при регистрации роутов придётся отказаться от "{controller}/{action=Index}/{id?}"

            return base.RouteAsync(context);
        }

        protected override Task OnRouteMatched(RouteContext context)
        {
            //вернём изначальный HttpContext.Request.Path
            context.HttpContext.Request.Path = context.HttpContext.Items["original-path"].ToString();

            //проверяем, что полученный из роута контроллер соответствует контроллеру,
            //полученному из типа страницы.
            //не уверен, что это нужно..
            //if (SavePathDataInHttpContext)
            //{
            //    var data = context.HttpContext.Items[RoutingKeys.PathData] as PathData;
            //    if (data != null)
            //    { 
            //        var controllerName = ControllerMapper.Map(data.AbstractItem);
            //        var parsedControllerName = (string)context.RouteData.Values["controller"];
            //        if (!string.Equals(parsedControllerName, controllerName, System.StringComparison.OrdinalIgnoreCase))
            //        {
            //            return Task.CompletedTask;
            //        }
            //    }
            //}
            return base.OnRouteMatched(context);
        }

        public override VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            //в RouteData должен быть CurrentItem
            if (!context.HttpContext.GetRouteData().DataTokens.ContainsKey(RoutingKeys.CurrentItem))
            {
                return null;
            }
            
            var page = (context.HttpContext.GetRouteData().DataTokens[RoutingKeys.CurrentItem] as IAbstractItem);
            if (page == null)
            {
                return null;
            }

            //проверим, что переданный контроллер соответствует типу страницы
            var controllerName = ControllerMapper.Map(page);
            if (!string.Equals(controllerName, context.Values["controller"]))
            {
                return null;
            }

            context.Values["controller"] = "--replace-with-path--";
            context.Values.Remove(RoutingKeys.CurrentItem);

            var vp = base.GetVirtualPath(context);

            if (vp == null)
                return null;

            vp.VirtualPath = vp.VirtualPath.Replace("--replace-with-path--", page.GetTrail().TrimStart('/'));

            return vp;
        }

        /// <summary>
        /// Определяем, что abstractItem принадлежит стартовой странице startPage
        /// </summary>
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
