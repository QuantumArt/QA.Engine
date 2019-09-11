using QA.DemoSite.Mssql.DAL;
using QA.DemoSite.Postgre.DAL;
using QA.DemoSite.Interfaces;
using QA.DemoSite.Interfaces.Dto;
using QA.DemoSite.Templates;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using QA.DotNetCore.Engine.Persistent.Interfaces;

namespace QA.DemoSite.Services
{
    public class FaqService : IFaqService
    {
        public FaqService(IConfiguration config, QpDataContext qpDataContext, PostgreQpDataContext postgreQpDataContext,
            ICacheProvider cacheProvider,
            CacheTagUtilities cacheTagUtilities)
        {
            QpDataContext = qpDataContext;
            PostgreQpDataContext = postgreQpDataContext;
            CacheProvider = cacheProvider;
            CacheTagUtilities = cacheTagUtilities;
            dbType = config.GetValue<DatabaseType>("dbType");
        }

        readonly DatabaseType dbType = DatabaseType.SqlServer;
        public QpDataContext QpDataContext { get; }
        public PostgreQpDataContext PostgreQpDataContext { get; }
        public ICacheProvider CacheProvider { get; }
        public CacheTagUtilities CacheTagUtilities { get; }

        public IEnumerable<FaqItemDto> GetItems(IEnumerable<int> ids)
        {
            return GetAll().Where(i => ids.Contains(i.Id)).ToList();
            //return QpDataContext.FaqItems.Where(i => ids.Contains(i.Id)).ToList().Select(Map).ToList();
        }

        private IEnumerable<FaqItemDto> GetAllCached()
        {
            return CacheProvider.GetOrAdd("FaqService.GetAll",
                CacheTagUtilities.Merge(CacheTags.FaqItem),
                TimeSpan.FromMinutes(60),
                () => GetAll());
        }

        private IEnumerable<FaqItemDto> GetAll()
        {
            if (dbType == DatabaseType.Postgres)
            {
                return PostgreQpDataContext.FaqItems.ToList().Select(Map).ToArray();
            }
            return QpDataContext.FaqItems.ToList().Select(Map).ToArray();
        }

        private FaqItemDto Map(Mssql.DAL.FaqItem faqItem)
        {
            return new FaqItemDto
            {
                Id = faqItem.Id,
                Answer = faqItem.Answer,
                Question = faqItem.Question,
                SortOrder = faqItem.SortOrder
            };
        }
        private FaqItemDto Map(Postgre.DAL.FaqItem faqItem)
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
