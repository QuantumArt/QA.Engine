using System;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using Quantumart.QPublishing.Database;

namespace QA.DotNetCore.Engine.OnScreen.Configuration
{
    public class OnScreenConfigurationOptions
    {
        public OnScreenSettings Settings { get; set; } = DefaultSettings;

        public DbConnectorSettings DbConnectorSettings { get; set; }

        /// <summary>
        /// Дефолтная конфигурация
        /// </summary>
        public static OnScreenSettings DefaultSettings = new OnScreenSettings
        {
            ApiApplicationNameInQp = "onscreen-api",
            AuthCookieLifetime = TimeSpan.FromMinutes(60),
            BackendSidQueryKey = "backend_sid",
            AuthCookieName = "qa_onscreen_token",
            AvailableFeatures = OnScreenFeatures.Widgets
        };
    }
}
