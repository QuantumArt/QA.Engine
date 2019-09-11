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
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;

namespace QA.DemoSite.Services
{
    public class FaqService : IFaqService
    {
        public FaqService(IDbContext qpDataContext,
            ICacheProvider cacheProvider,
            CacheTagUtilities cacheTagUtilities,
            QpSettings qpSettings)
        {
            QpDataContext = qpDataContext;
            CacheProvider = cacheProvider;
            CacheTagUtilities = cacheTagUtilities;
            if (!Enum.TryParse(qpSettings.DatabaseType, true, out DatabaseType dbType))
            {
                dbType = DatabaseType.SqlServer;
            }
        }

        readonly DatabaseType dbType = DatabaseType.SqlServer;
        public IDbContext QpDataContext { get; }
        public ICacheProvider CacheProvider { get; }
        public CacheTagUtilities CacheTagUtilities { get; }

        public IEnumerable<FaqItemDto> GetItems(IEnumerable<int> ids)
        {
            return GetAll().Where(i => ids.Contains(i.Id)).ToList();
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
                return (QpDataContext as PostgreQpDataContext).FaqItems.ToList().Select(Map).ToArray();
            }
            return (QpDataContext as QpDataContext).FaqItems.ToList().Select(Map).ToArray();
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
