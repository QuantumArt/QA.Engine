using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using System;

namespace QA.DotNetCore.Engine.CacheTags
{
    /// <summary>
    /// Инкапсулирует стандартное правило наименования кештегов для контентов QP. {ContentName}_{SiteId}_{Stage/Live}.
    /// </summary>
    public class DefaultQpContentCacheTagNamingProvider : IQpContentCacheTagNamingProvider
    {
        private readonly IMetaInfoRepository _metaInfoRepository;
        private readonly ICacheProvider _cacheProvider;

        public DefaultQpContentCacheTagNamingProvider(IMetaInfoRepository metaInfoRepository, ICacheProvider cacheProvider)
        {
            _metaInfoRepository = metaInfoRepository;
            _cacheProvider = cacheProvider;
        }

        public string Get(string contentName, int siteId, bool isStage)
        {
            //к кеш-тегам добавляю siteId, т.к. в теории на одной базе может быть несколько сайтов, и названия контентов могут совпадать
            return $"{contentName}_{siteId}_{(isStage ? "Stage" : "Live")}";
        }

        public string GetByNetName(string contentNetName, int siteId, bool isStage)
        {
            var contentInfo = _cacheProvider.GetOrAdd($"CacheTagCache_{contentNetName}_{siteId}", TimeSpan.FromDays(1), () => {
                return _metaInfoRepository.GetContent(contentNetName, siteId);
            });
            if (contentInfo == null)
                throw new ArgumentException($"Did not find content {contentNetName} in the site {siteId}");
            return Get(contentInfo.ContentName, siteId, isStage);
        }
    }
}
