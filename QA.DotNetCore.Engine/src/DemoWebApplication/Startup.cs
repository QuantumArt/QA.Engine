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
using QA.DotNetCore.Engine.AbTesting.Configuration;
using QA.DotNetCore.Engine.CacheTags;
using QA.DotNetCore.Engine.CacheTags.Configuration;
using QA.DotNetCore.Engine.OnScreen.Configuration;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using QA.DotNetCore.Engine.QpData.Configuration;
using QA.DotNetCore.Engine.Routing.Configuration;
using QA.DotNetCore.Engine.Routing.UrlResolve;
using QA.DotNetCore.Engine.Targeting.Configuration;
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
            var mvcBuilder = services.AddMvc(o => {
                o.EnableEndpointRouting = false;
            });
            services.AddLogging();
            services.AddMemoryCache();

            var qpSettings = Configuration.GetSection("QpSettings").Get<QpSettings>();

            services.AddSiteStructureEngine(options =>
            {
                options.UseQpSettings(qpSettings);
                options.TypeFinder.RegisterFromAssemblyContaining<RootPage, IAbstractItem>();
            });

            //services.AddSiteStructureEngineViaXml(options =>Load data for many-to-many fields in main content
            //{
            //    options.Settings.FilePath = @"C:\git\QA.Engine\QA.DotNetCore.Engine\src\DemoWebApplication\pages_and_widgets.xml";
            //    options.TypeFinder.RegisterFromAssemblyContaining<XmlRootPage, XmlAbstractItem>();
            //});

            services.AddAbTestServices(options =>
            {
                options.UseQpSettings(qpSettings);
            });

            var onScreenSettings = Configuration.GetSection("OnScreen").Get<OnScreenSettings>();
            services.AddOnScreenIntegration(mvcBuilder, options =>
            {
                options.AdminSiteBaseUrl = onScreenSettings.AdminSiteBaseUrl;
                options.AvailableFeatures = qpSettings.IsStage ? OnScreenFeatures.Widgets | OnScreenFeatures.AbTests : OnScreenFeatures.AbTests;
                options.UseQpSettings(qpSettings);
            });

            services.AddCacheTagServices(options =>
            {
                if (qpSettings.IsStage)
                {
                    options.InvalidateByMiddleware(@"^.*\/(__webpack.*|.+\.[a-zA-Z0-9]+)$");//отсекаем левые запросы для статики (для каждого сайта может настраиваться индивидуально)
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
            services.AddSingleton<DemoCultureRegionPossibleValuesProvider>();
            services.AddSingleton(Configuration.GetSection("UrlTokenConfig").Get<UrlTokenConfig>());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
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

            var qpSettings = Configuration.GetSection("QpSettings").Get<QpSettings>();
            app.UseOnScreenMode(qpSettings.CustomerCode);

            app.UseMvc(routes =>
            {
                routes.MapContentRoute("Route with custom params", "{controller}/{id}/{page}",
                    defaults: new RouteValueDictionary(new { action = "details" }),
                    constraints: new
                    {
                        page = @"^\d+$"
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
