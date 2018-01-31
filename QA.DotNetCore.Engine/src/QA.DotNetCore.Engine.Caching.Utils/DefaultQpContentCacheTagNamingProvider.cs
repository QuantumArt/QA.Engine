using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using System;

namespace QA.DotNetCore.Engine.Caching.Utils
{
    /// <summary>
    /// Инкапсулирует стандартное правило наименования кештегов для контентов QP. {ContentName}_{ContentId}_{Stage/Live}.
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

        public string Get(string contentName, int contentId, bool isStage)
        {
            //в кеш-тегах контентов использую ID контентов, т.к. в теории на одной базе может быть несколько сайтов, и названия контентов могут совпадать
            return $"{contentName}_{contentId}_{(isStage ? "Stage" : "Live")}";
        }

        public string GetByNetName(string contentNetName, int siteId, bool isStage)
        {
            var contentInfo = _cacheProvider.GetOrAdd($"CacheTagCache_{contentNetName}_{siteId}", TimeSpan.FromDays(1), () => {
                return _metaInfoRepository.GetContent(contentNetName, siteId);
            });
            if (contentInfo == null)
                throw new ArgumentException($"Did not find content {contentNetName} in the site {siteId}");
            return Get(contentInfo.ContentName, contentInfo.ContentId, isStage);
        }
    }
}
