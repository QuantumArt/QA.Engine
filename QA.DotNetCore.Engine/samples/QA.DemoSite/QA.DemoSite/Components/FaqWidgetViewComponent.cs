using Microsoft.AspNetCore.Mvc;
using QA.DemoSite.Interfaces;
using QA.DemoSite.Models.Widgets;
using QA.DemoSite.ViewModels;
using QA.DotNetCore.Engine.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DemoSite.Components
{
    public class FaqWidgetViewComponent : WidgetComponentBase<FaqWidget>
    {
        public FaqWidgetViewComponent(IFaqService faqService)
        {
            FaqService = faqService;
        }

        public IFaqService FaqService { get; }

        public override Task<IViewComponentResult> RenderAsync(FaqWidget widget, IDictionary<string, object> argumets)
        {
            return Task.FromResult<IViewComponentResult>(View(PrepareViewModel(widget)));
        }

        private FaqWidgetViewModel PrepareViewModel(FaqWidget widget)
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
