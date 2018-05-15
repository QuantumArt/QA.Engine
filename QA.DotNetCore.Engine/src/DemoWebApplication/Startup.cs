using DemoWebApplication.Templates;
using DemoWebSite.PagesAndWidgets;
using DemoWebSite.PagesAndWidgets.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.AbTesting.Configuration;
using QA.DotNetCore.Engine.CacheTags;
using QA.DotNetCore.Engine.CacheTags.Configuration;
using QA.DotNetCore.Engine.OnScreen.Configuration;
using QA.DotNetCore.Engine.QpData.Configuration;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.Engine.Routing;
using QA.DotNetCore.Engine.Routing.Configuration;
using QA.DotNetCore.Engine.Routing.UrlResolve;
using QA.DotNetCore.Engine.Targeting;
using QA.DotNetCore.Engine.Targeting.Configuration;
using Quantumart.QPublishing.Database;
using System;

namespace DemoWebApplication
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddMvc();
            services.AddLogging();
            services.AddMemoryCache();

            var qpSettings = Configuration.GetSection("QpSettings").Get<QpSettings>();
            var qpConnection = Configuration.GetConnectionString("QpConnection");

            services.AddSiteStructureEngine(options =>
            {
                options.QpConnectionString = qpConnection;
                options.QpSettings = qpSettings;
                options.QpSiteStructureSettings = Configuration.GetSection("QpSiteStructureSettings").Get<QpSiteStructureSettings>();
                options.TypeFinder.RegisterFromAssemblyContaining<RootPage, IAbstractItem>();
            });

            //services.AddSiteStructureEngineViaXml(options =>
            //{
            //    options.Settings.FilePath = @"C:\git\QA.Engine\QA.DotNetCore.Engine\src\DemoWebApplication\pages_and_widgets.xml";
            //    options.TypeFinder.RegisterFromAssemblyContaining<XmlRootPage, XmlAbstractItem>();
            //});

            services.AddAbTestServices(options => {
                //дублируются некоторые опции из AddSiteStructureEngine, потому что АБ-тесты могут быть или не быть независимо от структуры сайта
                options.QpConnectionString = qpConnection;
                options.AbTestingSettings.SiteId = qpSettings.SiteId;
                options.AbTestingSettings.IsStage = qpSettings.IsStage;
            });

            services.AddOnScreenIntegration(options =>
            {
                options.Settings.AdminSiteBaseUrl = Configuration.GetSection("OnScreen").Get<OnScreenSettings>().AdminSiteBaseUrl;
                options.Settings.SiteId = qpSettings.SiteId;
                options.Settings.IsStage = qpSettings.IsStage;
                options.Settings.AvailableFeatures = qpSettings.IsStage ? OnScreenFeatures.Widgets | OnScreenFeatures.AbTests : OnScreenFeatures.AbTests;
                options.DbConnectorSettings = new DbConnectorSettings
                {
                    ConnectionString = qpConnection,
                    IsLive = !qpSettings.IsStage
                };
            });

            services.AddCacheTagServices(options =>
            {
                if (qpSettings.IsStage)
                {
                    options.InvalidateByMiddleware(@"^.*\/.+\.[a-zA-Z0-9]+$");//отсекаем левые запросы для статики (для каждого сайта может настраиваться индивидуально)
                }
                else
                {
                    options.InvalidateByTimer(TimeSpan.FromSeconds(30));
                }
            });

            services.AddScoped<CacheTagUtilities>();

            //services.AddSingleton(typeof(DemoRegionTargetingProvider));
            //services.AddSingleton(typeof(DemoCultureTargetingProvider));
            services.AddSingleton(typeof(DemoRegionFilter));
            services.AddSingleton(typeof(DemoCultureFilter));

            services.AddTargeting();

            
            services.AddSingleton<UrlTokenResolverFactory>();
            services.AddSingleton<UrlTokenTargetingProvider>();
            services.AddScoped<DemoCultureRegionPossibleValuesProvider>();
            services.AddSingleton(Configuration.GetSection("UrlTokenConfig").Get<UrlTokenConfig>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            
            app.UseStaticFiles();

            app.UseCacheTagsInvalidation(trackers =>
            {
                trackers.RegisterScoped<QpContentCacheTracker>();
            });

            app.UseSiteStructure();

            app.UseTargeting((providers, possibleValues) =>
            {
                //targeting.Add<DemoCultureTargetingProvider>();
                //targeting.Add<DemoRegionTargetingProvider>();
                providers.RegisterSingleton<UrlTokenTargetingProvider>();
                possibleValues.RegisterSingleton<DemoCultureRegionPossibleValuesProvider>();
            });

            app.UseSiteSctructureFilters(filters =>
            {
                filters.RegisterSingleton<DemoRegionFilter>();
                filters.RegisterSingleton<DemoCultureFilter>();
            });

            app.UseOnScreenMode();

            app.UseMvc(routes =>
            {
                routes.MapContentRoute("Route with custom params", "{controller}/{id}/{page}",
                    defaults: new RouteValueDictionary(new { action = "details" }),
                    constraints: new { page = @"^\d+$"
                    });

                routes.MapContentRoute("default", "{controller}/{action=Index}/{id?}");

                //routes.MapGreedyContentRoute("blog bage with tail", "{controller}",
                //    defaults: new { controller = "blogpagetype", action = "Index" },
                //    constraints: new { controller = "blogpagetype" });

                routes.MapRoute("static controllers route", "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
