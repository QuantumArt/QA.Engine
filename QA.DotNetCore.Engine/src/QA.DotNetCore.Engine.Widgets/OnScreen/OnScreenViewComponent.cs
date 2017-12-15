using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Routing;
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
            var ai = ViewContext.GetCurrentItem();
            if (enabled)
                return new HtmlString($@"<div id='sidebarplaceholder'></div>
                <script type='text/javascript'>
                    window.onScreenAdminBaseUrl = '{_onScreenSettings.AdminSiteBaseUrl}';
                    window.currentPageId='{ai?.Id}';
                    window.siteId='{_onScreenSettings.SiteId}';
                    document.cookie = 'onscreen = true; expires = 0; path =/ ';
                 </script>
                <script src='{_onScreenSettings.AdminSiteBaseUrl}/dist/pmrpc.js' defer></script>
                <script src='{ _onScreenSettings.AdminSiteBaseUrl}/dist/onScreenLoader.js' defer></script>");
            return HtmlString.Empty;
        }
    }
}
