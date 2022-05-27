using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Routing
{
    public class SiteStructureRouteValueTransformer : DynamicRouteValueTransformer
    {
        public SiteStructureRouteValueTransformer(ITargetingFilterAccessor targetingFilterAccessor,
            IControllerMapper controllerMapper,
            ITailUrlResolver tailUrlResolver,
            IHeadUrlResolver headUrlResolver)
        {
            TargetingFilterAccessor = targetingFilterAccessor;
            ControllerMapper = controllerMapper;
            TailUrlResolver = tailUrlResolver;
            HeadUrlResolver = headUrlResolver;
        }

        public ITargetingFilterAccessor TargetingFilterAccessor { get; }
        public IControllerMapper ControllerMapper { get; }
        public ITailUrlResolver TailUrlResolver { get; }
        public IHeadUrlResolver HeadUrlResolver { get; }

        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            //стартовая страница, проставляется в RoutingMiddleware
            if (!(httpContext.Items[RoutingKeys.StartPage] is IStartPage startPage))
            {
                return ReturnValues();
            }

            var targetingFilter = TargetingFilterAccessor?.Get();//все зарегистрированные фильтры структуры сайта, объединенные в один

            //вычислим PathData текущего запроса - какая страница в структуре сайта должна быть активирована,
            //разбирая урл запроса, и сопоставляя сегменты со структурой сайта
            var path = httpContext.Request.Path;
            PathData data = CreatePathFinder().Find(path, startPage, targetingFilter, HeadUrlResolver);

            if (data == null)
            {
                return ReturnValues();
            }

            //вычислим какой контроллер должен вызываться
            var controllerName = ControllerMapper.Map(data.AbstractItem);
            if (string.IsNullOrEmpty(controllerName))
            {
                return ReturnValues();
            }

            //вычислим остальные route values из хвоста урла
            var routeValues = TailUrlResolver.ResolveRouteValues(data.RemainingUrl, controllerName);
            if (!routeValues.Any())
            {
                //не удалось определить из хвоста урла никаких route values
                return ReturnValues();
            }

            values["controller"] = controllerName;
            values[RoutingKeys.CurrentItem] = data.AbstractItem;

            foreach (var rvKey in routeValues.Keys)
            {
                values[rvKey] = routeValues[rvKey];
            }

            return ReturnValues();

            ValueTask<RouteValueDictionary> ReturnValues() => new ValueTask<RouteValueDictionary>(values);
        }

        private PathFinder CreatePathFinder()
        {
            return new PathFinder();
        }
    }
}
