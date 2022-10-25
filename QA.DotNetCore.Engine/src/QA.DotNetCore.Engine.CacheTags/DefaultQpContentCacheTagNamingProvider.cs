using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.CacheTags
{
    /// <summary>
    /// Инкапсулирует стандартное правило наименования кештегов для контентов QP. {ContentName}_{SiteId}_{Stage/Live}.
    /// </summary>
    public class DefaultQpContentCacheTagNamingProvider : IQpContentCacheTagNamingProvider
    {
        private readonly IMetaInfoRepository _metaInfoRepository;

        public DefaultQpContentCacheTagNamingProvider(IMetaInfoRepository metaInfoRepository)
        {
            _metaInfoRepository = metaInfoRepository;
        }

        public string Get(string contentName, int siteId, bool isStage)
        {
            //к кеш-тегам добавляю siteId, т.к. в теории на одной базе может быть несколько сайтов, и названия контентов могут совпадать
            return $"{contentName}_{siteId}_{(isStage ? "Stage" : "Live")}";
        }

        public string GetByNetName(string contentNetName, int siteId, bool isStage)
        {
            var contentInfo = _metaInfoRepository.GetContent(contentNetName, siteId);
            if (contentInfo == null)
            {
                throw new ArgumentException($"Did not find content {contentNetName} in the site {siteId}");
            }

            return Get(contentInfo.ContentName, siteId, isStage);
        }

        public Dictionary<int, string> GetByContentIds(int[] contentIds, int siteId, bool isStage)
        {
            var contentsInfo = _metaInfoRepository.GetContentsById(contentIds, siteId);
            return contentsInfo.ToDictionary(
                x => x.ContentId,
                x => Get(x.ContentName, siteId, isStage));
        }
    }
}
