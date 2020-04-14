using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.Persistent.Interfaces;
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

            if (String.IsNullOrWhiteSpace(options.AdminSiteBaseUrl))
                throw new Exception("Url for onscreen api is not configured.");

            if (options.SiteId == 0)
                throw new Exception("SiteId is not configured.");

            if (String.IsNullOrWhiteSpace(options.CustomerCode))
                throw new Exception("QP customer code for onscreen mode is not configured.");

            //добавляем настройки
            services.AddSingleton(new OnScreenSettings
            {
                AdminSiteBaseUrl = options.AdminSiteBaseUrl,
                IsStage = options.IsStage,
                SiteId = options.SiteId,
                CustomerCode = options.CustomerCode,
                ApiApplicationNameInQp = options.ApiApplicationNameInQp,
                AuthCookieName = options.AuthCookieName,
                AuthCookieLifetime = options.AuthCookieLifetime,
                AvailableFeatures = options.AvailableFeatures,
                BackendSidQueryKey = options.BackendSidQueryKey,
                OverrideAbTestStageModeCookieName = options.OverrideAbTestStageModeCookieName,
                PageIdQueryParamName = options.PageIdQueryParamName,
                SkipWidgetTypes = options.SkipWidgetTypes
            });

            //добавляем сервисы
            services.TryAddSingleton<IOnScreenContextProvider, OnScreenHttpContextProvider>();
            services.TryAddScoped(sp =>
            {
                var uow = sp.GetService<IUnitOfWork>();
                return new DBConnector(uow.Connection)
                {
                    IsStage = options.IsStage
                };
            });
            services.TryAddScoped<IAuthenticationService, AuthenticationService>();

            //делаем доступным для сайта view-компонент
            var onScreenAssembly = typeof(OnScreenViewComponent).Assembly;
            mvcBuilder.AddApplicationPart(onScreenAssembly);
            services.Configure<RazorViewEngineOptions>(o => { o.FileProviders.Add(new EmbeddedFileProvider(onScreenAssembly)); });
        }
    }
}
