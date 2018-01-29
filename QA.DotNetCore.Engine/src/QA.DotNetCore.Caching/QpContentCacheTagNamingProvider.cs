using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using System;

namespace QA.DotNetCore.Caching
{
    public class QpContentCacheTagNamingProvider : IQpContentCacheTagNamingProvider
    {
        private readonly IMetaInfoRepository _metaInfoRepository;
        private readonly ICacheProvider _cacheProvider;

        public QpContentCacheTagNamingProvider(IMetaInfoRepository metaInfoRepository)
        {
            _metaInfoRepository = metaInfoRepository;
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
