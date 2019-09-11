using QA.DotNetCore.Engine.Abstractions.OnScreen;
using System;

namespace QA.DotNetCore.Engine.OnScreen.Configuration
{
    public class OnScreenConfigurationOptions
    {
        public OnScreenSettings Settings { get; set; } = DefaultSettings;

        /// <summary>
        /// Дефолтная конфигурация
        /// </summary>
        public static OnScreenSettings DefaultSettings = new OnScreenSettings
        {
            ApiApplicationNameInQp = "onscreen-api",
            AuthCookieLifetime = TimeSpan.FromMinutes(60),
            BackendSidQueryKey = "backend_sid",
            AuthCookieName = "qa_onscreen_token",
            AvailableFeatures = OnScreenFeatures.Widgets,
            OverrideAbTestStageModeCookieName = "qa_onscreen_abtests_stage"
        };
    }
}
