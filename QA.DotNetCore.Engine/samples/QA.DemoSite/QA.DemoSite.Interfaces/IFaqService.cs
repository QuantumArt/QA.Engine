using QA.DemoSite.Interfaces.Dto;
using System;
using System.Collections.Generic;

namespace QA.DemoSite.Interfaces
{
    public interface IFaqService
    {
        IEnumerable<FaqItemDto> GetItems(IEnumerable<int> ids);
    }
}
