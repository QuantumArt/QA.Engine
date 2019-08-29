using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using Quantumart.QPublishing.Authentication;
using Quantumart.QPublishing.Database;
using System;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QP.ConfigurationService.Models;
using Dapper;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

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

            if (options.QpSettings == null)
                throw new Exception("QpSettings for OnScreen is not configured.");

            if (string.IsNullOrEmpty(options.QpSettings.ConfigurationServiceUrl)
                || string.IsNullOrEmpty(options.QpSettings.ConfigurationServiceToken))
                throw new Exception("ConfigurationServiceUrl and ConfigurationServiceToken for OnScreen is not configured.");

            services.AddSingleton(options.Settings);
            services.AddSingleton<IOnScreenContextProvider, OnScreenHttpContextProvider>();
            services.AddScoped<IUnitOfWork, UnitOfWork>(sp =>
            {
                DBConnector.ConfigServiceUrl = options.QpSettings.ConfigurationServiceUrl;
                DBConnector.ConfigServiceToken = options.QpSettings.ConfigurationServiceToken;
                CustomerConfiguration dbConfig = DBConnector.GetCustomerConfiguration(options.QpSettings.CustomerCode).Result;
                return new UnitOfWork(dbConfig.ConnectionString, dbConfig.DbType.ToString());
            });
            services.AddScoped(sp =>
            {
                var uow = sp.GetService<IUnitOfWork>();
                return new DBConnector(uow.Connection)
                {
                    IsStage = options.QpSettings.IsStage
                };
            });
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            var onScreenAssembly = typeof(OnScreenViewComponent).Assembly;
            mvcBuilder.AddApplicationPart(onScreenAssembly);
            services.Configure<RazorViewEngineOptions>(o => { o.FileProviders.Add(new EmbeddedFileProvider(onScreenAssembly)); });
        }
    }
}
