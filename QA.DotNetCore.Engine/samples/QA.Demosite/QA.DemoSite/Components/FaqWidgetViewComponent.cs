using Microsoft.AspNetCore.Mvc;
using QA.DemoSite.Interfaces;
using QA.DemoSite.Models.Widgets;
using QA.DemoSite.ViewModels;
using QA.DemoSite.ViewModels.Builders;
using QA.DotNetCore.Engine.Widgets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DemoSite.Components
{
    public class FaqWidgetViewComponent : WidgetComponentBase<FaqWidget>
    {
        public FaqWidgetViewComponent(FaqWidgetViewModelBuilder faqWidgetViewModelBuilder)
        {
            FaqWidgetViewModelBuilder = faqWidgetViewModelBuilder;
        }

        public FaqWidgetViewModelBuilder FaqWidgetViewModelBuilder { get; }

        public override Task<IViewComponentResult> RenderAsync(FaqWidget widget, IDictionary<string, object> argumets)
        {
            var vm = FaqWidgetViewModelBuilder.Build(widget);
            return Task.FromResult<IViewComponentResult>(View(vm));
        }
    }
}
