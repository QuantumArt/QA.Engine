using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.OnScreen.Configuration;
using QA.DotNetCore.Engine.Routing;
using System;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;

namespace QA.DotNetCore.Engine.OnScreen
{
    public class OnScreenViewComponent : ViewComponent
    {
        private readonly OnScreenSettings _onScreenSettings;
        private readonly QpSettings _qpSettings;


        public OnScreenViewComponent(OnScreenSettings onScreenSettings, QpSettings qpSettings)
        {
            _onScreenSettings = onScreenSettings;
            _qpSettings = qpSettings;
        }

        public IViewComponentResult Invoke()
        {
            var ctx = ((IOnScreenContextProvider)ViewContext.HttpContext.RequestServices.GetService(typeof(IOnScreenContextProvider)))?.GetContext();
            if (ctx == null)
                throw new InvalidOperationException("OnScreen context not found.");

            if (ctx.Enabled)
            {
                OnScreenViewModel model = new OnScreenViewModel
                {
                    StartPage = ViewContext.GetStartPage(),
                    AI = ViewContext.GetCurrentItem(),
                    OnScreenSettings = _onScreenSettings,
                    CustomerCode = _qpSettings.CustomerCode,
                    Ctx = ctx
                };
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
        public string CustomerCode { get; set; }
    }
}
