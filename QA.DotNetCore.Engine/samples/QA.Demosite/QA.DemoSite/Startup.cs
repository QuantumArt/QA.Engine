using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using QA.DemoSite.Mssql.DAL;
using QA.DemoSite.Interfaces;
using QA.DemoSite.Models.Pages;
using QA.DemoSite.Postgre.DAL;
using QA.DemoSite.Services;
using QA.DemoSite.Templates;
using QA.DemoSite.ViewModels.Builders;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.AbTesting.Configuration;
using QA.DotNetCore.Engine.CacheTags;
using QA.DotNetCore.Engine.CacheTags.Configuration;
using QA.DotNetCore.Engine.OnScreen.Configuration;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Configuration;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.Engine.Routing.Configuration;
using QA.DotNetCore.Engine.Targeting.Configuration;
using Quantumart.QPublishing.Database;
using System;
using System.Data.SqlClient;

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
            var mvc = services.AddMvc();// .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddLogging();
            services.AddSingleton<ILogger>(provider => provider.GetRequiredService<ILogger<Program>>());

            var qpSettings = Configuration.GetSection("QpSettings").Get<QpSettings>();

            if (!Enum.TryParse(Configuration.GetValue<string>("dbType"), true, out DatabaseType dbType))
            {
                dbType = DatabaseType.SqlServer;
            }
            var qpConnection = dbType == DatabaseType.SqlServer ? Configuration.GetConnectionString("DatabaseQP") : Configuration.GetConnectionString("DatabaseQPPostgre");

            //структура сайта виджетной платформы
            services.AddSiteStructureEngine(options =>
            {
                options.QpConnectionString = qpConnection;
                options.QpSettings = qpSettings;
                options.QpSiteStructureSettings = Configuration.GetSection("QpSiteStructureSettings").Get<QpSiteStructureSettings>();
                options.TypeFinder.RegisterFromAssemblyContaining<RootPage, IAbstractItem>();
            });


            //  ef контекст
            if (dbType == DatabaseType.Postgres)
            {
                services.AddScoped<IDbContext>(sp => PostgreQpDataContext.CreateWithStaticMapping(Postgre.DAL.ContentAccess.Live,
                  new NpgsqlConnection(qpConnection)));
            }
            else
            {
                services.AddScoped<IDbContext>(sp => QpDataContext.CreateWithStaticMapping(Mssql.DAL.ContentAccess.Live,
                    new SqlConnection(qpConnection)));
            }
            

            //сервисы слоя данных
            services.AddScoped<IFaqService, FaqService>();
            services.AddScoped<IBlogService, BlogService>();

            //сервисы построения view-model
            services.AddScoped<BlogPageViewModelBuilder>();
            services.AddScoped<FaqWidgetViewModelBuilder>();
            services.AddSingleton<MenuViewModelBuilder>();

            //подключение self-hosted аб-тестов
            services.AddTargeting();//чтобы аб-тесты работали нужно зарегистрировать сервисы таргетирования
            services.AddAbTestServices(options =>
            {
                //дублируются некоторые опции из AddSiteStructureEngine, потому что АБ-тесты могут быть или не быть независимо от структуры сайта
                options.QpConnectionString = qpConnection;
                options.AbTestingSettings.SiteId = qpSettings.SiteId;
                options.AbTestingSettings.IsStage = qpSettings.IsStage;
            });

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

            services.AddScoped(sp =>
            {
                var uow = sp.GetService<IUnitOfWork>();
                return new DBConnector(uow.Connection);
            });

           
            services.AddScoped<IUnitOfWork, UnitOfWork>(sp =>
            {               
                return new UnitOfWork(qpConnection, dbType.ToString());
            });
            //возможность работы с режимом onscreen
            services.AddOnScreenIntegration(mvc, options =>
            {
                options.Settings.AdminSiteBaseUrl = Configuration.GetSection("OnScreen").Get<OnScreenSettings>().AdminSiteBaseUrl;
                options.Settings.SiteId = qpSettings.SiteId;
                options.Settings.IsStage = qpSettings.IsStage;
                options.Settings.AvailableFeatures = OnScreenFeatures.Widgets | OnScreenFeatures.AbTests;
                options.DbConnectorSettings = new DbConnectorSettings
                {
                    ConnectionString = qpConnection,
                    IsLive = !qpSettings.IsStage
                };
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
            app.UseOnScreenMode();
            app.UseMvc(routes =>
            {
                routes.MapContentRoute("default", "{controller}/{action=Index}/{id?}");
                routes.MapRoute("static controllers route", "{controller}/{action=Index}/{id?}");
            });
        }
    }
}
