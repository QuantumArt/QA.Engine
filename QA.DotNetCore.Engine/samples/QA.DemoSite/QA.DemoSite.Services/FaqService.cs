using QA.DemoSite.DAL;
using QA.DemoSite.Interfaces;
using QA.DemoSite.Interfaces.Dto;
using QA.DemoSite.Templates;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DemoSite.Services
{
    public class FaqService : IFaqService
    {
        public FaqService(QpDataContext qpDataContext,
            ICacheProvider cacheProvider,
            CacheTagUtilities cacheTagUtilities)
        {
            QpDataContext = qpDataContext;
            CacheProvider = cacheProvider;
            CacheTagUtilities = cacheTagUtilities;
        }

        public QpDataContext QpDataContext { get; }
        public ICacheProvider CacheProvider { get; }
        public CacheTagUtilities CacheTagUtilities { get; }

        public IEnumerable<FaqItemDto> GetItems(IEnumerable<int> ids)
        {
            return GetAll().Where(i => ids.Contains(i.Id)).ToList();
            //return QpDataContext.FaqItems.Where(i => ids.Contains(i.Id)).ToList().Select(Map).ToList();
        }

        private IEnumerable<FaqItemDto> GetAll()
        {
            return CacheProvider.GetOrAdd("FaqService.GetAll",
                CacheTagUtilities.Merge(CacheTags.FaqItem),
                TimeSpan.FromMinutes(60),
                () => QpDataContext.FaqItems.ToList().Select(Map).ToList());
        }

        private FaqItemDto Map(FaqItem faqItem)
        {
            return new FaqItemDto
            {
                Id = faqItem.Id,
                Answer = faqItem.Answer,
                Question = faqItem.Question,
                SortOrder = faqItem.SortOrder
            };
        }
    }
}
