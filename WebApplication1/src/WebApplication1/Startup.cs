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
using WebApplication1.PageModel;
using WebApplication1.Routing;

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
            var storage = new AbstractItemStorage();
            storage.InitializeWith(new StartPage(1, "", "start page", 
                new TextPage(2, "about", "about company") { Text = "Some text"},
                new TextPage(3, "help", "Help page", 
                    new TextPage(4, "first", "First page") { Text = "Some first page text" },
                    new TextPage(5, "second", "Second page") { Text = "Some second page text"})));

            app.UseMvc(routes =>
            {
                IInlineConstraintResolver requiredService = routes.ServiceProvider.GetRequiredService<IInlineConstraintResolver>();

                //routes.Routes.Add(new ContentRoute(storage, routes.DefaultHandler, "default1", "test/{id}",
                //        new RouteValueDictionary(new { controller= "Home", action="contact"}),
                //        new RouteValueDictionary(null),
                //        new RouteValueDictionary(null),
                //    requiredService));

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
