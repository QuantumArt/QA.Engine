using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.Engine.Routing.Configuration;
using QA.DotNetCore.Engine.QpData.Configuration;
using QA.DotNetCore.Engine.Abstractions;
using NLog.Web;
using NLog.Extensions.Logging;
using QA.DemoSite.Models.Pages;
using QA.DotNetCore.Engine.QpData;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace QA.DemoSite
{
    public class Startup
    {
        private IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            _env = env;
            env.ConfigureNLog("nlog.config");
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc();// .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddLogging();
            services.AddSingleton<ILogger>(provider => provider.GetRequiredService<ILogger<Program>>());

            var qpSettings = Configuration.GetSection("QpSettings").Get<QpSettings>();
            var qpConnection = Configuration.GetConnectionString("DatabaseQP");

            var siteStructure = services.AddSiteStructureEngine(options =>
            {
                options.QpConnectionString = qpConnection;
                options.QpSettings = qpSettings;
                options.QpSiteStructureSettings = Configuration.GetSection("QpSiteStructureSettings").Get<QpSiteStructureSettings>();
                options.TypeFinder.RegisterFromAssemblyContaining<RootPage, IAbstractItem>();
            });

            services.AddSingleton<IAbstractItemStorageProvider, SimpleAbstractItemStorageProvider>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            loggerFactory.AddNLog();
            var logger = loggerFactory.CreateLogger(typeof(Startup));

            app.UseExceptionHandler(options =>
            {
                options.Run(context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "text/html";
                    var ex = context.Features.Get<IExceptionHandlerFeature>();
                    if (ex != null)
                    {
                        logger.LogError(1000, ex.Error, "Exception occurred which has not been handled in the MVC application");
                    }
                    return Task.CompletedTask;
                });
            });

            app.UseExceptionHandler("/Error/ServerError");

            app.UseStaticFiles();
            app.UseSiteStructure();

            app.UseMvc(routes =>
            {
                routes.MapContentRoute("Route with custom params", "{controller}/{id}/{page}",
                    defaults: new RouteValueDictionary(new { action = "details" }));

                routes.MapContentRoute("default", "{controller}/{action=Index}/{id?}");
                routes.MapRoute("static controllers route", "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
