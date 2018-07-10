using QA.DemoSite.DAL;
using QA.DemoSite.Interfaces;
using QA.DemoSite.Interfaces.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DemoSite.Services
{
    public class FaqService : IFaqService
    {
        public FaqService(QpDataContext qpDataContext)
        {
            QpDataContext = qpDataContext;
        }

        public QpDataContext QpDataContext { get; }

        public IEnumerable<FaqItemDto> GetItems(IEnumerable<int> ids)
        {
            return QpDataContext.FaqItems.Where(i => ids.Contains(i.Id)).ToList().Select(Map).ToList();
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
