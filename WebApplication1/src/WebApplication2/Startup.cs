using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Common.PageModel;
using Common.Routing;
using Common.Persistent.Data;
using Common.Persistent;
using Common.Persistent.Dapper;
using Common.Widgets;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace WebApplication2
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
                _ => {
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

            // initialize fake structure
            var storage = app.ApplicationServices.GetService<IAbstractItemStorageProvider>().Get();



            app.UseMvc(routes =>
            {
                IInlineConstraintResolver requiredService = routes.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();

                routes.MapRoute("static controllers route1", "test/{action=Index}/{id?}", new { controller = "somestatic" });

                routes.Routes.Add(new ContentRoute(storage, routes.DefaultHandler, "Route with custom params", "{controller}/{action=Details}/{id}/{page}",
                    new RouteValueDictionary(null),
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
