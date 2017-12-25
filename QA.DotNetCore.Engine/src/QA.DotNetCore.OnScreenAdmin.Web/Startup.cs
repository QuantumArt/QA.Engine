using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using Microsoft.AspNetCore.Http;
using Quantumart.QPublishing.Database;
using Quantumart.QPublishing.Authentication;
using QA.DotNetCore.OnScreenAdmin.Web.Auth;
using Microsoft.AspNetCore.Authorization;

namespace QA.DotNetCore.OnScreenAdmin.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddMemoryCache();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var dbConnectorSettings = Configuration.GetSection("DbConnectorSettings").Get<DbConnectorSettings>();
            dbConnectorSettings.ConnectionString = Configuration.GetConnectionString("QpConnection");
            services.AddSingleton(typeof(DbConnectorSettings), dbConnectorSettings);
            services.AddScoped<DBConnector>();
            services.AddScoped<AuthenticationService>();


            services.AddScoped<IUnitOfWork, UnitOfWork>(sp => new UnitOfWork(Configuration.GetConnectionString("QpConnection")));
            services.AddScoped<IMetaInfoRepository, MetaInfoRepository>();
            services.AddScoped<INetNameQueryAnalyzer, NetNameQueryAnalyzer>();
            services.AddScoped<IItemDefinitionRepository, ItemDefinitionRepository>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = QpAuthDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = QpAuthDefaults.AuthenticationScheme;
            }).AddQpAuth(authOptions =>
            {
                authOptions.Settings = Configuration.GetSection("QpAuthSettings").Get<QpAuthSettings>();
            });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicy(
                    new List<IAuthorizationRequirement>
                    {
                        new QpUserRequirement()
                    },
                    new[] { QpAuthDefaults.AuthenticationScheme });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true,
                    HotModuleReplacementEndpoint = "/__webpack_hmr"
                });
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCors(builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                   name: "spa-fallback",
                   defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
