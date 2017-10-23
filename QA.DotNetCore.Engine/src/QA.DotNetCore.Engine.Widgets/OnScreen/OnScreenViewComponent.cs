using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Widgets.Configuration;

namespace QA.DotNetCore.Engine.Widgets.OnScreen
{
    public class OnScreenViewComponent : ViewComponent
    {
        private readonly OnScreenSettings _onScreenSettings;

        public OnScreenViewComponent(OnScreenSettings onScreenSettings)
        {
            _onScreenSettings = onScreenSettings;
        }

        public HtmlString Invoke()
        {
            var enabled = ViewContext.HttpContext.OnScreenEditEnabled();
            if (enabled)
                return new HtmlString($@"<div id='sidebarplaceholder'></div><script type='text/javascript'>window.onScreenAdminBaseUrl = '{_onScreenSettings.AdminSiteBaseUrl}';</script><script src='{ _onScreenSettings.AdminSiteBaseUrl}/onScreenLoader.js' defer></script>");
            return HtmlString.Empty;
        }
    }
}
