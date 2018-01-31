using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Interfaces;
using QA.DotNetCore.Engine.Persistent.Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Caching.Utils.Configuration
{
    public static class MvcServiceCollectionExtensions
    {
        /// <summary>
        /// Добавление сервисов для работы кештегов в IServiceCollection
        /// </summary>
        /// <param name="services">коллекция сервисов</param>
        public static void AddCacheTagServices(this IServiceCollection services)
        {
            services.AddSingleton<ICacheTagWatcher, CacheTagWatcher>();
            services.AddSingleton<IQpContentCacheTagNamingProvider, DefaultQpContentCacheTagNamingProvider>();
            services.AddScoped<QpContentCacheTracker>();
            services.AddScoped<IContentModificationRepository, ContentModificationRepository>();
            services.AddSingleton<ICacheTrackersAccessor, CacheTrackersAccessor>();
            services.AddSingleton<CacheTagsInvalidationConfigurator>();
        }
    }
}
