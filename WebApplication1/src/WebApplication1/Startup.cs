using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Common.PageModel;
using Common.Routing;
using Common.Widgets;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Common.Persistent;
using Common.Persistent.Dapper;
using Common.Persistent.Data;

namespace WebApplication1
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
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

            services.Add(new ServiceDescriptor(typeof(AbstractItemActivator), new AbstractItemActivator()));

            services.Add(new ServiceDescriptor(typeof(IUnitOfWork),
                _ =>
                {
                    return new UnitOfWork(Configuration.GetConnectionString("QpConnection"));
                },
                ServiceLifetime.Scoped));

            services.Add(new ServiceDescriptor(typeof(IAbstractItemStorageProvider),
                typeof(QpAbstractItemStorageProvider),
                ServiceLifetime.Scoped));

            //services.Add(new ServiceDescriptor(typeof(AbstractItemStorage), _ => {
            //    return services.GetService<IUnitOfWork>();
            //}, ServiceLifetime.Scoped));
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
            var storage = app.ApplicationServices.GetService<IAbstractItemStorageProvider>().Get(66643);

            app.UseMvc(routes =>
            {
                IInlineConstraintResolver requiredService = routes.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();

                routes.MapRoute("static controllers route1", "test/{action=Index}/{id?}", new { controller = "somestatic" });

                routes.Routes.Add(new ContentRoute(storage, routes.DefaultHandler, "Route with custom params", "{controller}/{id}/{page}",
                    new RouteValueDictionary(new { action = "details" }),
                    new RouteValueDictionary(null),
                    new RouteValueDictionary(null),
                    requiredService));


                routes.Routes.Add(new ContentRoute(storage, routes.DefaultHandler, "default", "{controller}/{action=Index}/{id?}",
                    new RouteValueDictionary(null),
                    new RouteValueDictionary(null),
                    new RouteValueDictionary(null),
                    requiredService));

                routes.MapRoute("static controllers route", "{controller}/{action=Index}/{id?}");

            });
        }
    }
}
