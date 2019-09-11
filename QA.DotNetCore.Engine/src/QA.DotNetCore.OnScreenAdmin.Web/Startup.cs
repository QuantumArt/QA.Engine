using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.CacheTags;
using QA.DotNetCore.Engine.CacheTags.Configuration;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.Engine.QpData.Settings;
using QA.DotNetCore.OnScreenAdmin.Web.Auth;
using Quantumart.QPublishing.Authentication;
using Quantumart.QPublishing.Database;
using System;
using System.Collections.Generic;
using QP.ConfigurationService.Models;
using Swashbuckle.AspNetCore.Swagger;

namespace QA.DotNetCore.OnScreenAdmin.Web
{
    public class Startup
    {
        const string SWAGGER_VERSION = "v1";
        const string SWAGGER_TITLE = "OnScreen Web Api";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSwaggerGen(o =>
            {
                o.SwaggerDoc(SWAGGER_VERSION, new Info
                {
                    Title = SWAGGER_TITLE,
                    Version = SWAGGER_VERSION
                });
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                o.IncludeXmlComments(xmlPath);
            });
            services.AddMemoryCache();
            services.AddSingleton<ICacheProvider, VersionedCacheCoreProvider>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped(sp =>
            {
                var uow = sp.GetService<IUnitOfWork>();
                return new DBConnector(uow.Connection);
            });
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>(sp =>
            {
                var httpContextAccessor = sp.GetService<IHttpContextAccessor>();
                if (!httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Customer-Code", out var customerCode))
                {
                    throw new Exception("Customer-Code header must be provided.");
                }
                var config = Configuration.GetSection("ConfigurationService").Get<ConfigurationServiceConfig>();
                DBConnector.ConfigServiceUrl = config.Url;
                DBConnector.ConfigServiceToken = config.Token;
                CustomerConfiguration dbConfig = DBConnector.GetCustomerConfiguration(customerCode.ToString()).Result;
                return new UnitOfWork(dbConfig.ConnectionString, dbConfig.DbType.ToString());
            });
            services.AddScoped<IMetaInfoRepository, MetaInfoRepository>();
            services.AddScoped<INetNameQueryAnalyzer, NetNameQueryAnalyzer>();
            services.AddScoped<IItemDefinitionRepository, ItemDefinitionRepository>();
            services.AddScoped<IAbTestRepository, AbTestRepository>();

            var qpUrlResolverCacheSettings = new QpSchemeCacheSettings { CachePeriod = new TimeSpan(0, 1, 0) };
            services.AddSingleton(typeof(QpSchemeCacheSettings), qpUrlResolverCacheSettings);
            services.AddScoped<IQpUrlResolver, QpUrlResolver>();
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

            services.AddCacheTagServices(options => options.InvalidateByMiddleware(@"^(?!\/api\/).+$"));//инвалидировать только если запрос начинается с /api/

            services.AddHealthChecks();

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

            app.UseCacheTagsInvalidation(trackers =>
            {
                trackers.RegisterScoped<QpContentCacheTracker>();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseHealthChecks("/ready");

            app.UseSwagger();
            app.UseSwaggerUI(o =>
            {
                o.SwaggerEndpoint("/swagger/v1/swagger.json", $"{SWAGGER_TITLE} {SWAGGER_VERSION}");
            });

        }
    }
}
