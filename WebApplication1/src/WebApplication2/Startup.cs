using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Common.PageModel;
using Common.Routing;

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
            services.Add(new ServiceDescriptor(typeof(AbstractItemStorage), new AbstractItemStorage()));
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
            var storage = app.ApplicationServices.GetService<AbstractItemStorage>();
            InitializeFakeSiteStructure(storage);



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

        private static void InitializeFakeSiteStructure(AbstractItemStorage storage)
        {
            storage.InitializeWith(new StartPage(1, "", "Site main page",
                                new TextPage(2, "about", "about company") { Text = "Some text" },
                                new TextPage(3, "help", "Help page",
                                    new TextPage(4, "first", "First page") { Text = "Some first page text" },
                                    new TextPage(5, "second", "Second page") { Text = "Some second page text" },
                                    new TextPage(6, "another", "Another page",
                                        new TextPage(11, "test", "Test page")
                                        )
                                    { Text = "Some page text" }
                                    ),
                                new TextPage(7, "help-new", "Help page",
                                    new TextPage(8, "first", "First page") { Text = "Some first page text" },
                                    new TextPage(9, "second", "Second page") { Text = "Some second page text" },
                                    new TextPage(10, "another", "Another page") { Text = "Some page text" }
                                    )
                                )
                            );
        }
    }
}
