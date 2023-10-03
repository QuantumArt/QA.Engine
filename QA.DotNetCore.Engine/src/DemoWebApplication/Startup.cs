using DemoWebApplication.Templates;
using DemoWebSite.PagesAndWidgets;
using DemoWebSite.PagesAndWidgets.Pages;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.AbTesting.Configuration;
using QA.DotNetCore.Engine.CacheTags;
using QA.DotNetCore.Engine.CacheTags.Configuration;
using QA.DotNetCore.Engine.OnScreen.Configuration;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using QA.DotNetCore.Engine.QpData.Configuration;
using QA.DotNetCore.Engine.Routing.Configuration;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching;
using QA.DotNetCore.Engine.Targeting.Configuration;
using QA.DotNetCore.Engine.Targeting.Filters;
using QA.DotNetCore.Engine.Targeting.TargetingProviders;
using System.Collections.Generic;
using System.Threading.Tasks;
using static QA.DotNetCore.Engine.Routing.Configuration.ControllerEndpointRouteBuilderExtensions;

namespace DemoWebApplication
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
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
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddLogging();
            services.AddMemoryCache();

            //начиная с 3.1 по умолчанию будет активирован EndpointRouting
            //чтобы работать в старом режиме роутинга нужно передать o => o.EnableEndpointRouting = false
            var mvcBuilder = services.AddMvc();

            var qpSettings = Configuration.GetSection("QpSettings").Get<QpSettings>();

            //подключение необходимых сервисов для того, чтобы сайт мог использовать структуру сайта виджетной платформы
            services.AddSiteStructureEngine(options =>
            {
                //[обязательные]
                //настройки для взаимодействия с QP (строка подключения, SiteId, live/stage итд), в котором хранится структура сайта
                options.UseQpSettings(qpSettings);

                //[обязательная]
                //регистрация сборки, которая содержит все классы страниц и виджетов, созданных разработчиком
                options.TypeFinder.RegisterFromAssemblyContaining<RootPage, IAbstractItem>();

                //[опционально]
                //настройка шаблонов для "головы" урла, в которых могут содержаться некоторые токены (например регион, культура итд).
                //по умолчанию, если не указывать эту настройку, виджетная платформа считает, что таких токенов нет.
                //если в UrlHeadPatterns заданы какие-то токены, то можно через RegisterUrlHeadTokenPossibleValues зарегистрировать один
                //или несколько классов-реализаций IHeadTokenPossibleValuesProvider
                options.UrlHeadPatterns = Configuration.GetSection("UrlTokenConfig:HeadPatterns").Get<List<HeadUrlMatchingPattern>>();
                options.RegisterUrlHeadTokenPossibleValues<DictionariesPossibleValuesProvider>();

                //[опционально]
                //[ТОЛЬКО для Endpoint routing]
                //настройка шаблонов для "хвоста" урла
                //DefaultUrlTailPattern - шаблон, который будет использоваться по умолчанию для всех контроллеров, если не указывать явно, то будет {action=Index}/{id?}
                //UrlTailPatternsByControllers - шаблоны, которые будут использоваться только для определенных контроллеров
                options.UrlTailPatternsByControllers = Configuration.GetSection("UrlTokenConfig:TailByControllers")
                    .Get<Dictionary<string, List<TailUrlMatchingPattern>>>();

                //[опционально]
                //конвенция того, как ItemDefinition сопоставляется c .net классами страниц и виджетов, которые заводит разработчик
                //по умолчанию, если не указывать эту настройку, используется конвенция Name
                //Name - предполагает совпадение поля TypeName у ItemDefinition и имени класса .Net
                //Attribute - предполагает наличие атрибута у класса .Net, в котором задаётся дискриминатор ItemDefinition 
                options.ItemDefinitionConvention = ItemDefinitionConvention.Name;

                //[опционально]
                //конвенция того, как .net классам страниц соответствуют контроллеры
                //по умолчанию, если не указывать эту настройку, используется конвенция Name
                //Name - предполагает, что контроллер должен называться также как класс страницы (TextPage -> TextPageController)
                //Attribute - предполагает, что контроллер должен быть помечен атрибутом SiteStructureControllerAttribute, в котором должен быть указан тип страницы
                options.ControllerMapperConvention = ControllerMapperConvention.Name;

                //[опционально]
                //конвенция того, как .net классам виджетов соответствуют viewcomponent-ы
                //по умолчанию, если не указывать эту настройку, используется конвенция Name
                //Name - предполагает, что компонент должен называться также как тип виджета (TextWidget -> TextWidgetViewComponent)
                //Attribute - предполагает, что компонент должен быть помечен атрибутом SiteStructureComponentAttribute, в котором должен быть указан тип виджета
                options.ComponentMapperConvention = ComponentMapperConvention.Name;

                //[опционально]
                //настройка того, нужно ли загружать ли в нетипизированную коллекцию всех свойств страниц и виджетов поля из AbstractItem
                //по умолчанию - true
                options.LoadAbstractItemFieldsToDetailsCollection = true;


                options.DictionarySettings = Configuration.GetSection("DictionariesConfig").Get<List<DictionarySettings>>();
            });

            //подключение необходимых сервисов для работы self-contained аб-тестов
            services.AddAbTestServices(options =>
            {
                //[обязательные] настройки для взаимодействия с QP (строка подключения, SiteId, live/stage итд), в котором хранятся аб-тесты
                options.UseQpSettings(qpSettings);
            });

            //подключение к сайту возможности работать в режиме OnScreen
            var onScreenSettings = Configuration.GetSection("OnScreen").Get<OnScreenSettings>();
            services.AddOnScreenIntegration(mvcBuilder, options =>
            {
                //[обязательная]
                //настройка с урлом компонента OnScreen Api
                options.AdminSiteBaseUrl = onScreenSettings.AdminSiteBaseUrl;

                //[обязательная]
                //настройка фич, которыми можно управлять через OnScreen
                options.AvailableFeatures = qpSettings.IsStage ? OnScreenFeatures.Widgets | OnScreenFeatures.AbTests : OnScreenFeatures.AbTests;

                //[обязательные]
                //настройки для взаимодействия с QP
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

            //подключение сервисов для таргетирования
            services.AddTargeting();

            //регистрация автосгенерированного класса CacheTagUtilities (см. CacheTags.tt)
            services.AddScoped<CacheTagUtilities>();

            //регистрация всех используемых фильтров структуры сайта
            services.AddSingleton(typeof(RegionFilter));
            services.AddSingleton(typeof(CultureFilter));

            //регистрация всех используемых IHeadTokenPossibleValuesProvider
            services.AddScoped<DictionariesPossibleValuesProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();

            //мидлвара для инвалидации кештегов
            app.UseCacheTagsInvalidation();

            //мидлвара, вычисляющая для запроса значения таргетирования
            //необходимо, чтобы было подключено services.AddTargeting
            app.UseTargeting(providers =>
            {
                //регистрация одного или нескольких ITargetingProvider
                //UrlTokenTargetingProvider - уже реализованный ITargetingProvider, который получает значения таргетирования на основе UrlHeadPatterns
                providers.Register<UrlTokenTargetingProvider>();
                providers.Register<CultureTargetingProvider>();
                providers.Register<RegionTargetingProvider>();
            });

            //мидлвара, добавляющая структуру сайта в pipeline запроса
            //необходимо, чтобы было подключено services.AddSiteStructureEngine
            app.UseSiteStructure();

            //регистрируем фильтры для структуры сайта
            //необходимо, чтобы было подключено services.AddTargeting
            app.UseSiteStructureFilters(filters =>
            {
                filters.Register<RegionFilter>();
                filters.Register<CultureFilter>();
            });

            //мидлвара aspnet core для endpoint routing (EnableEndpointRouting = true)
            app.UseRouting();

            var qpSettings = Configuration.GetSection("QpSettings").Get<QpSettings>();

            //мидлвара, проверяющая возможность работы в режиме onscreen, добавляющая OnScreenContext в pipeline запроса
            //необходимо, чтобы было подключено services.AddOnScreenIntegration
            app.UseOnScreenMode(qpSettings.CustomerCode);

            //мидлвара aspnet core для endpoint routing (EnableEndpointRouting = true)
            app.UseEndpoints(endpoints =>
            {
                //регистрируем endpoint для запросов в модуль аб-тестов
                //необходимо, чтобы было подключено services.AddAbTestServices
                endpoints.MapAbtestEndpointRoute();

                //регистрируем endpoint для всех страниц сайта
                //необходимо, чтобы было подключено services.AddSiteStructureEngine
                endpoints.MapSiteStructureControllerRoute();
            });


            //мидлвара aspnet core для старой версии routing (EnableEndpointRouting = false)

            //app.UseMvc(routes =>
            //{
            //      MapContentRoute регистрирует роут для всех страниц сайта
            //      необходимо, чтобы было подключено services.AddSiteStructureEngine
            //      то, что в endpoint routing задаётся с помощью DefaultUrlTailPattern и UrlTailPatternsByControllers здесь задаётся в виде роутов
            //      необходимо, чтобы было подключено services.AddSiteStructureEngine
            //    routes.MapContentRoute("default", "{controller}/{action=Index}/{id?}");
            //    routes.MapRoute("static controllers route", "{controller}/{action=Index}/{id?}");
            //});

            app.Use(next => context =>
            {
                //сюда попадаем, если ни один endpoint не подошёл
                context.Response.WriteAsync("404!");
                return Task.CompletedTask;
            });
        }
    }
}
