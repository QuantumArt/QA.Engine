using QA.DemoSite.Interfaces;
using QA.DemoSite.Interfaces.PagesAndWidgets;
using System;
using System.Linq;

namespace QA.DemoSite.ViewModels.Builders
{
    public class FaqWidgetViewModelBuilder
    {
        public FaqWidgetViewModelBuilder(IFaqService faqService)
        {
            FaqService = faqService;
        }

        public IFaqService FaqService { get; }

        public FaqWidgetViewModel Build(IFaqWidget widget)
        {
            var vm = new FaqWidgetViewModel { Id = widget.Id, Header = widget.Header };
            if (widget.Questions.Any())
            {
                vm.Items.AddRange(FaqService.GetItems(widget.Questions)
                    .OrderBy(i => i.SortOrder.GetValueOrDefault(Int32.MaxValue))
                    .Select(i => new FaqWidgetItemViewModel { Id = i.Id, Answer = i.Answer, Question = i.Question }));
            }
            return vm;
        }
    }
}
