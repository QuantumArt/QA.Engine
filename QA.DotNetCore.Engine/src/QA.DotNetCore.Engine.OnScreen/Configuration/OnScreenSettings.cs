using QA.DotNetCore.Engine.Abstractions.OnScreen;
using System;

namespace QA.DotNetCore.Engine.OnScreen.Configuration
{
    public class OnScreenSettings
    {
        public string AdminSiteBaseUrl { get; set; }
        public int SiteId { get; set; }
        public bool IsStage { get; set; }
        public OnScreenFeatures AvailableFeatures { get; set; }
        public string AuthCookieName { get; set; }
        public string BackendSidQueryKey { get; set; }
        public TimeSpan AuthCookieLifetime { get; set; }
        public string ApiApplicationNameInQp { get; set; }
        public string OverrideAbTestStageModeCookieName { get; set; }
    }
}
