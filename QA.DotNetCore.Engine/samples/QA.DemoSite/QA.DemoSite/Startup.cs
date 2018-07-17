using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QA.DemoSite.DAL;
using QA.DemoSite.Interfaces;
using QA.DemoSite.Models.Pages;
using QA.DemoSite.Services;
using QA.DemoSite.Templates;
using QA.DemoSite.ViewModels.Builders;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.CacheTags;
using QA.DotNetCore.Engine.CacheTags.Configuration;
using QA.DotNetCore.Engine.QpData.Configuration;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.Engine.Routing.Configuration;
using System;

namespace QA.DemoSite
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();// .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddLogging();
            services.AddSingleton<ILogger>(provider => provider.GetRequiredService<ILogger<Program>>());

            var qpSettings = Configuration.GetSection("QpSettings").Get<QpSettings>();
            var qpConnection = Configuration.GetConnectionString("DatabaseQP");

            //структура сайта виджетной платформы
            var siteStructure = services.AddSiteStructureEngine(options =>
            {
                options.QpConnectionString = qpConnection;
                options.QpSettings = qpSettings;
                options.QpSiteStructureSettings = Configuration.GetSection("QpSiteStructureSettings").Get<QpSiteStructureSettings>();
                options.TypeFinder.RegisterFromAssemblyContaining<RootPage, IAbstractItem>();
            });

            //ef контекст
            services.AddScoped(sp => QpDataContext.CreateWithStaticMapping(ContentAccess.Live,
                new System.Data.SqlClient.SqlConnection(qpConnection),
                true));

            //сервисы слоя данных
            services.AddScoped<IFaqService, FaqService>();
            services.AddScoped<IBlogService, BlogService>();

            //сервисы построения view-model
            services.AddScoped<BlogPageViewModelBuilder>();
            services.AddScoped<FaqWidgetViewModelBuilder>();
            services.AddSingleton<MenuViewModelBuilder>();

            //работа с кеш-тэгами
            services.AddScoped<CacheTagUtilities>();
            services.AddCacheTagServices(options =>
            {
                if (qpSettings.IsStage)
                {
                    options.InvalidateByMiddleware(@"^.*\/.+\.[a-zA-Z0-9]+$");//отсекаем левые запросы для статики (для каждого сайта может настраиваться индивидуально)
                }
                else
                {
                    options.InvalidateByTimer(TimeSpan.FromSeconds(30));
                }
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger logger)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            app.UseCacheTagsInvalidation(invalidation =>
            {
                invalidation.RegisterScoped<QpContentCacheTracker>();
            });
            app.UseSiteStructure();

            app.UseMvc(routes =>
            {
                routes.MapContentRoute("default", "{controller}/{action=Index}/{id?}");
                routes.MapRoute("static controllers route", "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
