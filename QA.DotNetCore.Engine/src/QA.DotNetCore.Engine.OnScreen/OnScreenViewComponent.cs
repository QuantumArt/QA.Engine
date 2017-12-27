using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Routing;
using QA.DotNetCore.Engine.OnScreen.Configuration;
using QA.DotNetCore.Engine.Abstractions.OnScreen;

namespace QA.DotNetCore.Engine.OnScreen
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
            var ctx = ((IOnScreenContextProvider)ViewContext.HttpContext.RequestServices.GetService(typeof(IOnScreenContextProvider)))?.GetContext();
            var ai = ViewContext.GetCurrentItem();
            if (ctx.Enabled)
            { 
                return new HtmlString($@"<div id='sidebarplaceholder'></div>
                <script type='text/javascript'>
                    window.onScreenAdminBaseUrl = '{_onScreenSettings.AdminSiteBaseUrl}';
                    window.currentPageId='{ai?.Id}';
                    window.siteId='{_onScreenSettings.SiteId}';
                    window.onScreenFeatures = '{ctx.Features}';
                    window.onScreenTokenCookieName = '{_onScreenSettings.AuthCookieName}';
                 </script>
                <script src='{_onScreenSettings.AdminSiteBaseUrl}/dist/pmrpc.js' defer></script>
                <script src='{ _onScreenSettings.AdminSiteBaseUrl}/dist/onScreenLoader.js' defer></script>");
            }
            return HtmlString.Empty;
        }
    }
}
