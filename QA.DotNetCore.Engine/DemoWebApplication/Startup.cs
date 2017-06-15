using DemoWebSite.PagesAndWidgets;
using DemoWebSite.PagesAndWidgets.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.Configure<QpSiteStructureSettings>(Configuration.GetSection("QpSiteStructureSettings"));
            services.Configure<QpSettings>(Configuration.GetSection("QpSettings"));
            services.Configure<SiteMode>(Configuration.GetSection("SiteMode"));

            // Add framework services.
            services.AddMvc();

            services.AddMemoryCache();

            services.Add(new ServiceDescriptor(typeof(ICacheProvider), typeof(VersionedCacheCoreProvider), ServiceLifetime.Singleton));
            services.Add(new ServiceDescriptor(typeof(IViewComponentInvokerFactory), typeof(WidgetViewComponentInvokerFactory), ServiceLifetime.Scoped));

            services.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(UnitOfWork), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IAbstractItemRepository), typeof(AbstractItemRepository), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IMetaInfoRepository), typeof(MetaInfoRepository), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IItemDefinitionRepository), typeof(ItemDefinitionRepository), ServiceLifetime.Scoped));

            services.Add(new ServiceDescriptor(typeof(ITypeFinder), provider => new SingleAssemblyTypeFinder(new RootPage()), ServiceLifetime.Singleton));
            services.Add(new ServiceDescriptor(typeof(IItemDefinitionProvider), typeof(NameConventionalItemDefinitionProvider), ServiceLifetime.Scoped));
            
            services.Add(new ServiceDescriptor(typeof(IComponentMapper), typeof(NameConventionalComponentMapper), ServiceLifetime.Singleton));
            services.Add(new ServiceDescriptor(typeof(IControllerMapper), typeof(NameConventionalControllerMapper), ServiceLifetime.Singleton));
            services.Add(new ServiceDescriptor(typeof(IAbstractItemFactory), typeof(AbstractItemFactory), ServiceLifetime.Scoped));
            
            services.Add(new ServiceDescriptor(typeof(IQpUrlResolver), typeof(QpUrlResolver), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IAbstractItemStorageBuilder), typeof(QpAbstractItemStorageBuilder), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IAbstractItemStorageProvider), typeof(SimpleAbstractItemStorageProvider), ServiceLifetime.Scoped));
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

            app.UseMvc(routes =>
            {
                IInlineConstraintResolver requiredService = routes.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();
                IControllerMapper controllerMapper = routes.ServiceProvider.GetRequiredService<IControllerMapper>();

                routes.Routes.Add(new ContentRoute(controllerMapper, routes.DefaultHandler, "Route with custom params", "{controller}/{id}/{page}",
                    new RouteValueDictionary(new { action = "details" }),
                    new RouteValueDictionary(null),
                    new RouteValueDictionary(null),
                    requiredService));


                routes.Routes.Add(new ContentRoute(controllerMapper, routes.DefaultHandler, "default", "{controller}/{action=Index}/{id?}",
                    new RouteValueDictionary(null),
                    new RouteValueDictionary(null),
                    new RouteValueDictionary(null),
                    requiredService));

                routes.MapRoute("static controllers route", "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
