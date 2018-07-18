using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using Quantumart.QPublishing.Authentication;
using Quantumart.QPublishing.Database;
using System;

namespace QA.DotNetCore.Engine.OnScreen.Configuration
{
    public class OnScreenConfigurator
    {
        public OnScreenConfigurator(IServiceCollection services, IMvcBuilder mvcBuilder, Action<OnScreenConfigurationOptions> setupAction)
        {
            var options = new OnScreenConfigurationOptions();

            setupAction?.Invoke(options);

            if (options.Settings == null)
                throw new Exception("OnScreen settings is not configured.");

            if (String.IsNullOrWhiteSpace(options.Settings.AdminSiteBaseUrl))
                throw new Exception("Url for onscreen admin site is not configured.");

            if (options.Settings.SiteId == 0)
                throw new Exception("SiteId for onscreen api is not configured.");

            if (options.DbConnectorSettings == null)
                throw new Exception("DbConnectorSettings for OnScreen is not configured.");

            if (options.DbConnectorSettings.ConnectionString == null)
                throw new Exception("ConnectionString in DbConnectorSettings for OnScreen is not configured.");

            services.AddSingleton(options.Settings);
            services.AddSingleton(options.DbConnectorSettings);
            services.AddSingleton<IOnScreenContextProvider, OnScreenHttpContextProvider>();
            services.AddScoped<DBConnector>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            var onScreenAssembly = typeof(OnScreenViewComponent).Assembly;
            mvcBuilder.AddApplicationPart(onScreenAssembly);
            services.Configure<RazorViewEngineOptions>(o => { o.FileProviders.Add(new EmbeddedFileProvider(onScreenAssembly)); });
        }
    }
}
