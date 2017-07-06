using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Routing
{
    public abstract class ContentControllerBase<T> : Controller
        where T : class, IAbstractItem
    {
        public virtual T CurrentItem { get { return ControllerContext.GetCurrentItem<T>(); } }

        public virtual IStartPage StartPage { get { return ControllerContext.GetStartPage(); } }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData[RoutingKeys.CurrentItem] = CurrentItem;
            ViewData[RoutingKeys.StartPage] = StartPage;

            base.OnActionExecuting(context);
        }
        
    }
}
