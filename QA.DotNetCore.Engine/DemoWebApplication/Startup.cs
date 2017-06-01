using DemoWebSite.PagesAndWidgets;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using QA.DotNetCore.Engine.Routing;
using QA.DotNetCore.Engine.Widgets;

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
            // Add framework services.
            services.AddMvc();

            services.Add(new ServiceDescriptor(typeof(IViewComponentInvokerFactory),
                typeof(WidgetViewComponentInvokerFactory),
                ServiceLifetime.Scoped));

            services.Add(new ServiceDescriptor(typeof(IComponentMapper), new DemoComponentMapper()));

            services.Add(new ServiceDescriptor(typeof(IControllerMapper), new DemoControllerMapper()));

            services.Add(new ServiceDescriptor(typeof(IAbstractItemFactory), new DemoAbstractItemFactory()));

            services.Add(new ServiceDescriptor(typeof(IUnitOfWork),
                _ =>
                {
                    return new UnitOfWork(Configuration.GetConnectionString("QpConnection"));
                },
                ServiceLifetime.Scoped));

            services.Add(new ServiceDescriptor(typeof(IAbstractItemStorageProvider),
                typeof(QpAbstractItemStorageProvider),
                ServiceLifetime.Scoped));
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

            // initialize structure
            var storage = app.ApplicationServices.GetService<IAbstractItemStorageProvider>().Get();

            app.UseMvc(routes =>
            {
                IInlineConstraintResolver requiredService = routes.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();
                IControllerMapper controllerMapper = routes.ServiceProvider.GetRequiredService<IControllerMapper>();

                routes.Routes.Add(new ContentRoute(storage, controllerMapper, routes.DefaultHandler, "Route with custom params", "{controller}/{id}/{page}",
                    new RouteValueDictionary(new { action = "details" }),
                    new RouteValueDictionary(null),
                    new RouteValueDictionary(null),
                    requiredService));


                routes.Routes.Add(new ContentRoute(storage, controllerMapper, routes.DefaultHandler, "default", "{controller}/{action=Index}/{id?}",
                    new RouteValueDictionary(null),
                    new RouteValueDictionary(null),
                    new RouteValueDictionary(null),
                    requiredService));

                routes.MapRoute("static controllers route", "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
