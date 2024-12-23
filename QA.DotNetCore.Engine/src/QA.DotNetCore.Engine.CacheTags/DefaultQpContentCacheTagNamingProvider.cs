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
        private IUnitOfWork _unitOfWork;

        public DefaultQpContentCacheTagNamingProvider(IMetaInfoRepository metaInfoRepository)
        {
            _metaInfoRepository = metaInfoRepository;
        }

        public void SetUnitOfWork(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _metaInfoRepository.SetUnitOfWork(unitOfWork);
        }

        protected string CustomerCode => _unitOfWork?.CustomerCode ?? "current";

        public string Get(string contentName, int siteId, bool isStage)
        {
            //к кеш-тегам добавляю siteId, т.к. в теории на одной базе может быть несколько сайтов, и названия контентов могут совпадать
            return $"{CustomerCode}_{contentName}_{siteId}_{(isStage ? "Stage" : "Live")}";
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

        public Dictionary<string, string> GetByContentNetNames(string[] contentNetName, int siteId, bool isStage)
        {
            var contentsInfo = _metaInfoRepository.GetContents(contentNetName, siteId);
            return contentsInfo.ToDictionary(
                x => x.ContentNetName,
                x => Get(x.ContentName, x.SiteId, isStage));
        }

        public Dictionary<int, string> GetByContentIds(int[] contentIds, bool isStage)
        {
            var contentsInfo = _metaInfoRepository.GetContentsById(contentIds);
            return contentsInfo.ToDictionary(
                x => x.ContentId,
                x => Get(x.ContentName, x.SiteId, isStage));
        }
    }
}
