using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.CacheTags;
using QA.DotNetCore.Engine.CacheTags.Configuration;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using QA.DotNetCore.Engine.QpData.Configuration;
using QA.DotNetCore.Engine.Targeting.Configuration;
using QA.DotNetCore.Engine.Targeting.Factories;
using QA.WidgetPlatform.Api.Services;
using QA.WidgetPlatform.Api.Services.Abstract;
using System.Text.Json.Serialization;

namespace QA.WidgetPlatform.Api.Infrastructure
{
    public static class ConfigureServicesExt
    {
        public static IServiceCollection ConfigureBaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Widget Platform API", Version = "v1" });
                foreach (var xmlFile in Directory.GetFiles(AppContext.BaseDirectory, "*.xml"))
                {
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    options.IncludeXmlComments(xmlPath);
                }
            });

            services.AddMemoryCache();
            var qpSettings = configuration.GetSection("QpSettings").Get<QpSettings>();
            services.AddSiteStructure(options =>
            {
                options.UseQpSettings(qpSettings);
            });

            //подключение сервисов для работы кештегов
            var cacheTagBuilder = services
                .AddCacheTagServices()
                .WithCacheTrackers(trackers =>
                {
                    //регистрация одного или нескольких ICacheTagTracker
                    //QpContentCacheTracker - уже реализованный ICacheTagTracker, который работает на базе механизма CONTENT_MODIFICATION из QP
                    trackers.Register<QpContentCacheTracker>();
                });

            //[обязательная]
            //настройка стратегии инвалидации по кештегам
            if (qpSettings.IsStage)
            {
                //при каждом запросе запускать все зарегистрированные ICacheTagTracker,
                //чтобы получить все теги по которым нужно сбросить кеш, и сбросить его
                _ = cacheTagBuilder.WithInvalidationByMiddleware(@"^.*\/(__webpack.*|.+\.[a-zA-Z0-9]+)$");//отсекаем левые запросы для статики (для каждого сайта может настраиваться индивидуально)
            }
            else
            {
                //по таймеру запускать все зарегистрированные ICacheTagTracker,
                //чтобы получить все теги по которым нужно сбросить кеш, и сбросить его
                _ = cacheTagBuilder.WithInvalidationByTimer();
            }

            services.AddScoped<ISiteStructureService, SiteStructureService>();
            services.AddApiTargeting(configuration);


            return services;
        }
    }
}
