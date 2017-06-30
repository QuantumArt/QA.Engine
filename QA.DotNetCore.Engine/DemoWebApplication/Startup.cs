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
using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.Reflection;
using QA.DotNetCore.Engine.Routing;
using QA.DotNetCore.Engine.Targeting;
using QA.DotNetCore.Engine.Targeting.Filters;
using QA.DotNetCore.Engine.Widgets;
using static QA.DotNetCore.Engine.QpData.SiteStructureEngineConfiguratorExtensions;
using static QA.DotNetCore.Engine.Routing.SiteStructureEngineConfiguratorExtensions;
using static QA.DotNetCore.Engine.Widgets.SiteStructureEngineConfiguratorExtensions;

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
            services.AddSingleton(_ => Configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add framework services.
            services.AddMvc();
            
            services.AddMemoryCache();

            services.AddSingleton<ICacheProvider, VersionedCacheCoreProvider>();

            services.AddSiteStructureEngine(Configuration)
                .AddWidgetInvokerFactory()
                .AddSingleAssemblyTypeFinder(new RootPage())
                .AddItemDefinitionProvider(ItemDefinitionConvention.Name)
                .AddComponentMapper(ComponentMapperConvention.Name)
                .AddControllerMapper(ControllerMapperConvention.Name);

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

            app.UseSiteSctructure(routes =>
            {
                routes.MapContentRoute("Route with custom params", "{controller}/{id}/{page}", new RouteValueDictionary(new { action = "details" }));
                routes.MapContentRoute("default", "{controller}/{action=Index}/{id?}");
                routes.MapRoute("static controllers route", "{controller}/{action=Index}/{id?}");
            });

            app.UseTargeting(targeting =>
            {
                targeting.Add<DemoCultureTargetingProvider>();
                targeting.Add<DemoRegionTargetingProvider>();
            });

            app.UseSiteSctructureFilters(cfg =>
            {
                cfg.Add<DemoRegionFilter>();
                cfg.Add<DemoCultureFilter>();
            });
        }
    }
}
