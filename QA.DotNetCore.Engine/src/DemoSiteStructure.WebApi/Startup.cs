using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Engine.CacheTags;
using QA.DotNetCore.Engine.CacheTags.Configuration;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.QpData.Configuration;
using QA.DotNetCore.Engine.QpData.Interfaces;

namespace DemoSiteStructure.WebApi
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
            var qpSettings = Configuration.GetSection("QpSettings").Get<QpSettings>();

            services.AddMvc();
            services.AddLogging();
            services.AddMemoryCache();

            services.AddSiteStructure(options =>
            {
                options.UseQpSettings(qpSettings);
            });

            //подключение сервисов для работы кештегов
            services.AddCacheTagServices(options =>
            {
                //[обязательная]
                //настройка стратегии инвалидации по кештегам
                if (qpSettings.IsStage)
                {
                    //при каждом запросе запускать все зарегистрированные ICacheTagTracker,
                    //чтобы получить все теги по которым нужно сбросить кеш, и сбросить его
                    options.InvalidateByMiddleware(@"^.*\/(__webpack.*|.+\.[a-zA-Z0-9]+)$");//отсекаем левые запросы для статики (для каждого сайта может настраиваться индивидуально)
                }
                else
                {
                    //по таймеру запускать все зарегистрированные ICacheTagTracker,
                    //чтобы получить все теги по которым нужно сбросить кеш, и сбросить его
                    options.InvalidateByTimer(TimeSpan.FromSeconds(30));
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //мидлвара для инвалидации кештегов
            //необходимо, чтобы было подключено services.AddCacheTagServices
            app.UseCacheTagsInvalidation(trackers =>
            {
                //регистрация одного или нескольких ICacheTagTracker
                //QpContentCacheTracker - уже реализованный ICacheTagTracker, который работает на базе механизма CONTENT_MODIFICATION из QP
                trackers.Register<QpContentCacheTracker>();
            });

            app.UseRouting();

            app.UseEndpoints(routes =>
            {
                routes.MapControllers();
            });
        }
    }
}
