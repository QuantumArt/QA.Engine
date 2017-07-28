using DemoNetFrameworkWebApp.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Configuration;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.Engine.Routing.Configuration;

namespace DemoNetFrameworkWebApp
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
            services.AddMvc();
            services.AddMemoryCache();

            services.AddSingleton<ICacheProvider, VersionedCacheCoreProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSiteStructureEngine(options => {
                options.QpSettings = Configuration.GetSection("QpSettings").Get<QpSettings>();
                options.QpConnectionString = Configuration.GetConnectionString("QpConnection");
                options.TypeFinder.RegisterFromAssemblyContaining<RootPage, IAbstractItem>();
            });
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

            app.UseMvc(routes =>
            {
                routes.MapContentRoute("Route with custom params", "{controller}/{id}/{page}", new { action = "details" });
                routes.MapContentRoute("default", "{controller}/{action=Index}/{id?}");
                routes.MapRoute("static controllers route", "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
