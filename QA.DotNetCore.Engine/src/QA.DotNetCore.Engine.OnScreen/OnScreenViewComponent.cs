using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.OnScreen.Configuration;
using QA.DotNetCore.Engine.Routing;
using System;

namespace QA.DotNetCore.Engine.OnScreen
{
    public class OnScreenViewComponent : ViewComponent
    {
        private readonly OnScreenSettings _onScreenSettings;

        public OnScreenViewComponent(OnScreenSettings onScreenSettings)
        {
            _onScreenSettings = onScreenSettings;
        }

        public IViewComponentResult Invoke()
        {
            var ctx = ((IOnScreenContextProvider)ViewContext.HttpContext.RequestServices.GetService(typeof(IOnScreenContextProvider)))?.GetContext();
            if (ctx == null)
                throw new InvalidOperationException("OnScreen context not found.");

            if (ctx.Enabled)
            {
                OnScreenViewModel model = new OnScreenViewModel();
                model.StartPage = ViewContext.GetStartPage();
                model.AI = ViewContext.GetCurrentItem();
                model.OnScreenSettings = _onScreenSettings;
                model.Ctx = ctx;
                return View(model);
            }
            return Content(string.Empty);
        }
    }

    public class OnScreenViewModel
    {
        public IStartPage StartPage { get; internal set; }
        public IAbstractItem AI { get; internal set; }
        public OnScreenSettings OnScreenSettings { get; set; }
        public OnScreenContext Ctx { get; set; }
    }
}
