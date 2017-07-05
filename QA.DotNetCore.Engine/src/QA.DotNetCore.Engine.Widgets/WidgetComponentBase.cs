using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Widgets
{
    public abstract class WidgetComponentBase<T> : ViewComponent
        where T : IAbstractWidget
    {
        protected T CurrentItem { get; private set; }
        public Task<IViewComponentResult> InvokeAsync(T currentItem)
        {            
            CurrentItem = currentItem;
            return RenderAsync(currentItem, ViewComponentContext.Arguments);
        }
        public abstract Task<IViewComponentResult> RenderAsync(T currentItem, IDictionary<string, object> argumets);
    }
}
