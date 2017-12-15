using DemoWebSite.PagesAndWidgets;
using DemoWebSite.PagesAndWidgets.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.AbTesting.Configuration;
using QA.DotNetCore.Engine.QpData.Configuration;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.Engine.Routing.Configuration;
using QA.DotNetCore.Engine.Targeting.Configuration;
using QA.DotNetCore.Engine.Widgets.Configuration;
using QA.DotNetCore.Engine.Widgets.OnScreen;
using QA.DotNetCore.Engine.AbTesting;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Dapper;

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
            //services.Add<IRouter, DemoWebApplication.Debugging.MvcRouteHandler>();
            services.AddMemoryCache();

            services.AddSingleton<ICacheProvider, VersionedCacheCoreProvider>();

            services.AddSiteStructureEngine(options =>
            {
                options.QpConnectionString = Configuration.GetConnectionString("QpConnection");
                options.QpSettings = Configuration.GetSection("QpSettings").Get<QpSettings>();
                options.QpSiteStructureSettings = Configuration.GetSection("QpSiteStructureSettings").Get<QpSiteStructureSettings>();
                options.TypeFinder.RegisterFromAssemblyContaining<RootPage, IAbstractItem>();
                //options.QpSiteStructureSettings.LoadAbstractItemFieldsToDetailsCollection = false;
            });

            services.AddAbTestServices(options => {
                //дублируются некоторые опции из AddSiteStructureEngine, потому что АБ-тесты могут быть или не быть независимо от структуры сайта
                options.QpConnectionString = Configuration.GetConnectionString("QpConnection");
                options.AbTestingSettings.SiteId = Configuration.GetSection("QpSettings").Get<QpSettings>().SiteId;
                options.AbTestingSettings.IsStage = Configuration.GetSection("QpSettings").Get<QpSettings>().IsStage;
            });

            services.AddOnScreenServices(options =>
            {
                options.AdminSiteBaseUrl = Configuration.GetSection("OnScreen").Get<OnScreenSettings>().AdminSiteBaseUrl;
                options.SiteId = Configuration.GetSection("QpSettings").Get<QpSettings>().SiteId;
            });

            services.AddSingleton(typeof(DemoRegionTargetingProvider));
            services.AddSingleton(typeof(DemoCultureTargetingProvider));
            services.AddSingleton(typeof(DemoRegionFilter));
            services.AddSingleton(typeof(DemoCultureFilter));

            services.AddTargeting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSiteStructure();

            app.UseTargeting(targeting =>
            {
                targeting.Add<DemoCultureTargetingProvider>();
                targeting.Add<DemoRegionTargetingProvider>();
            });

            app.UseOnScreenMode();

            app.UseSiteSctructureFilters(cfg =>
            {
                cfg.Add<DemoRegionFilter>();
                cfg.Add<DemoCultureFilter>();
            });

            app.UseMvc(routes =>
            {
                routes.MapContentRoute("Route with custom params", "{controller}/{id}/{page}",
                    defaults: new RouteValueDictionary(new { action = "details" }),
                    constraints: new { page = @"^\d+$"
                    });

                routes.MapContentRoute("default", "{controller}/{action=Index}/{id?}");

                routes.MapGreedyContentRoute("blog bage with tail", "{controller}",
                    defaults: new { controller = "blogpagetype", action = "Index" },
                    constraints: new { controller = "blogpagetype" });

                routes.MapRoute("static controllers route", "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
