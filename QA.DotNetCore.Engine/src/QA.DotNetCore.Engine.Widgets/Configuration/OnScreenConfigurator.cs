using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Widgets.OnScreen;
using System;

namespace QA.DotNetCore.Engine.Widgets.Configuration
{
    public class OnScreenConfigurator
    {
        public OnScreenConfigurator(IServiceCollection services, Action<OnScreenSettings> setupAction)
        {
            var settings = new OnScreenSettings();

            setupAction?.Invoke(settings);

            if (String.IsNullOrWhiteSpace(settings.AdminSiteBaseUrl))
                throw new Exception("Url for onscreen admin site is not configured.");

            services.AddSingleton(settings);
            services.AddSingleton<IOnScreenContextProvider, OnScreenContextProvider>();
        }
    }
}
