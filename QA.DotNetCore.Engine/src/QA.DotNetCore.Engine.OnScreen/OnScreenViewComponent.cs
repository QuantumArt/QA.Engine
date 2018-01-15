using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Routing;
using QA.DotNetCore.Engine.OnScreen.Configuration;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using System;
using System.Text;

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
            if (ctx == null)
                throw new InvalidOperationException("OnScreen context not found.");

            if (ctx.Enabled)
            {
                var startPage = ViewContext.GetStartPage();
                var ai = ViewContext.GetCurrentItem();
                var markup = new StringBuilder($@"<div id='sidebarplaceholder'></div>
                <script type='text/javascript'>
                    window.onScreenAdminBaseUrl = '{_onScreenSettings.AdminSiteBaseUrl}';
                    window.currentPageId='{ai?.Id}';
                    window.startPageId='{startPage?.Id}';
                    window.siteId='{_onScreenSettings.SiteId}';
                    window.isStage={_onScreenSettings.IsStage.ToString().ToLower()};
                    window.onScreenFeatures = '{ctx.Features}';
                    window.onScreenTokenCookieName = '{_onScreenSettings.AuthCookieName}';
                    window.onScreenOverrideAbTestStageModeCookieName = '{_onScreenSettings.OverrideAbTestStageModeCookieName}';
                 </script>
                <script src='{_onScreenSettings.AdminSiteBaseUrl}/dist/pmrpc.js' defer></script>
                <script src='{_onScreenSettings.AdminSiteBaseUrl}/dist/onScreenLoader.js' defer></script>");

                if (ctx.HasFeature(OnScreenFeatures.AbTests))
                {
                    markup.AppendLine($"<script src='{_onScreenSettings.AdminSiteBaseUrl}/dist/cookies.js' defer></script>");
                    markup.AppendLine($"<script src='{_onScreenSettings.AdminSiteBaseUrl}/dist/onScreenAbTestApi.js' defer></script>");
                }

                return new HtmlString(markup.ToString());
            }
            return HtmlString.Empty;
        }
    }
}
