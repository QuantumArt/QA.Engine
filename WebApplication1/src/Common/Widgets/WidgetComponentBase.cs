﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.PageModel;
using Microsoft.AspNetCore.Mvc;

namespace Common.Widgets
{
    public abstract class WidgetComponentBase<T> : ViewComponent
        where T : AbstractWidget
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
