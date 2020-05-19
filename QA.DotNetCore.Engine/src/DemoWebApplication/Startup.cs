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
using QA.DotNetCore.Engine.Routing;
using QA.DotNetCore.Engine.Routing.Configuration;
using QA.DotNetCore.Engine.Routing.UrlResolve;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching;
using QA.DotNetCore.Engine.Targeting.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static QA.DotNetCore.Engine.Routing.Configuration.ControllerEndpointRouteBuilderExtensions;

namespace DemoWebApplication
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
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
            var mvcBuilder = services.AddMvc(o =>
            {
                //o.EnableEndpointRouting = false;
            }).AddRazorRuntimeCompilation();
            services.AddLogging();
            services.AddMemoryCache();

            var qpSettings = Configuration.GetSection("QpSettings").Get<QpSettings>();

            services.AddSiteStructureEngine(options =>
            {
                options.UseQpSettings(qpSettings);
                options.TypeFinder.RegisterFromAssemblyContaining<RootPage, IAbstractItem>();
                options.UrlHeadPatterns = Configuration.GetSection("UrlTokenConfig:HeadPatterns").Get<List<HeadUrlMatchingPattern>>();
                options.UrlTailPatternsByControllers = Configuration.GetSection("UrlTokenConfig:TailByControllers")
                    .Get<Dictionary<string, List<TailUrlMatchingPattern>>>();
                options.RegisterUrlHeadTokenPossibleValues<DemoCultureRegionPossibleValuesProvider>();
            });

            //services.AddSiteStructureEngineViaXml(options =>
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

            services.AddSingleton<UrlTokenTargetingProvider>();
            services.AddSingleton<DemoCultureRegionPossibleValuesProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseCacheTagsInvalidation(trackers =>
            {
                trackers.Register<QpContentCacheTracker>();
            });

            app.UseSiteStructure();

            app.UseTargeting(providers =>
            {
                //targeting.Add<DemoCultureTargetingProvider>();
                //targeting.Add<DemoRegionTargetingProvider>();
                providers.Register<UrlTokenTargetingProvider>();
                //possibleValues.RegisterSingleton<DemoCultureRegionPossibleValuesProvider>();
            });

            app.UseSiteStructureFilters(filters =>
            {
                filters.Register<DemoRegionFilter>();
                filters.Register<DemoCultureFilter>();
            });

            app.UseRouting();

            var qpSettings = Configuration.GetSection("QpSettings").Get<QpSettings>();
            app.UseOnScreenMode(qpSettings.CustomerCode);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapAbtestEndpointRoute();
                endpoints.MapSiteStructureControllerRoute();
            });

            app.Use(next => context =>
            {
                //сюда попадаем, если ни один endpoint не подошёл
                context.Response.WriteAsync("404!");
                return Task.CompletedTask;
            });


            //app.UseMvc(routes =>
            //{
            //    routes.MapContentRoute("Route with custom params", "{controller}/{id}/{page}",
            //        defaults: new RouteValueDictionary(new { action = "details" }),
            //        constraints: new
            //        {
            //            page = @"^\d+$"
            //        });

            //    routes.MapContentRoute("default", "{controller}/{action=Index}/{id?}");

            //    //routes.MapGreedyContentRoute("blog bage with tail", "{controller}",
            //    //    defaults: new { controller = "blogpagetype", action = "Index" },
            //    //    constraints: new { controller = "blogpagetype" });

            //    routes.MapRoute("static controllers route", "{controller}/{action=Index}/{id?}");
            //});
        }
    }
}
