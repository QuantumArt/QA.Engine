using DemoWebSite.PagesAndWidgets;
using DemoWebSite.PagesAndWidgets.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.Engine.Reflection;
using QA.DotNetCore.Engine.Routing;
using QA.DotNetCore.Engine.Routing.Mappers;
using QA.DotNetCore.Engine.Widgets;
using QA.DotNetCore.Engine.Widgets.Mappers;
using QA.DotNetCore.Engine.Targeting;
using QA.DotNetCore.Engine.Targeting.Filters;
using QA.DotNetCore.Engine.Abstractions.Targeting;

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

            services.Configure<QpSiteStructureSettings>(Configuration.GetSection("QpSiteStructureSettings"));
            services.Configure<QpSettings>(Configuration.GetSection("QpSettings"));
            services.Configure<SiteMode>(Configuration.GetSection("SiteMode"));

            // Add framework services.
            services.AddMvc();

            services.AddMemoryCache();

            services.AddSingleton<ICacheProvider, VersionedCacheCoreProvider>();
            services.AddScoped<IViewComponentInvokerFactory, WidgetViewComponentInvokerFactory>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAbstractItemRepository, AbstractItemRepository>();
            services.AddScoped<IMetaInfoRepository, MetaInfoRepository>();
            services.AddScoped<IItemDefinitionRepository, ItemDefinitionRepository>();

            services.AddSingleton<ITargetingContext, HttpTargetingContext>();
            services.AddSingleton<ITargetingProvidersConfigurator, TargetingProvidersConfigurator>();
            services.AddSingleton(typeof(DemoRegionTargetingProvider));
            services.AddSingleton(typeof(DemoCultureTargetingProvider));

            services.AddSingleton<ITargetingFilterAccessor, TargetingFilterAccessor>();
            services.AddSingleton<ITargetingFiltersConfigurator, TargetingFiltersConfigurator>();
            services.AddSingleton(typeof(DemoRegionFilter));
            services.AddSingleton(typeof(DemoCultureFilter));

            services.Add(new ServiceDescriptor(typeof(ITypeFinder), provider => new SingleAssemblyTypeFinder(new RootPage()), ServiceLifetime.Singleton));
            services.AddScoped<IItemDefinitionProvider, NameConventionalItemDefinitionProvider>();

            services.AddSingleton<IComponentMapper, NameConventionalComponentMapper>();
            services.AddSingleton<IControllerMapper, NameConventionalControllerMapper>();
            services.AddScoped<IAbstractItemFactory, AbstractItemFactory>();

            services.AddScoped<IQpUrlResolver, QpUrlResolver>();
            services.AddScoped<IAbstractItemStorageBuilder, QpAbstractItemStorageBuilder>();
            services.AddScoped<IAbstractItemStorageProvider, SimpleAbstractItemStorageProvider>();
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

            app.UseMiddleware<RoutingMiddleware>();

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

            app.UseMvc(routes =>
            {
                IInlineConstraintResolver requiredService = routes.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();
                IControllerMapper controllerMapper = routes.ServiceProvider.GetRequiredService<IControllerMapper>();
                ITargetingFilterAccessor targetingAccessor = routes.ServiceProvider.GetRequiredService<ITargetingFilterAccessor>();

                routes.Routes.Add(new ContentRoute(controllerMapper, targetingAccessor, routes.DefaultHandler, "Route with custom params", "{controller}/{id}/{page}",
                    new RouteValueDictionary(new { action = "details" }),
                    new RouteValueDictionary(null),
                    new RouteValueDictionary(null),
                    requiredService));


                routes.Routes.Add(new ContentRoute(controllerMapper, targetingAccessor, routes.DefaultHandler, "default", "{controller}/{action=Index}/{id?}",
                    new RouteValueDictionary(null),
                    new RouteValueDictionary(null),
                    new RouteValueDictionary(null),
                    requiredService));

                routes.MapRoute("static controllers route", "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
