using System.Collections.Generic;

namespace QA.DemoSite.ViewModels
{
    public class FaqWidgetViewModel
    {
        public FaqWidgetViewModel()
        {
            Items = new List<FaqWidgetItemViewModel>();
        }
        public int Id { get; set; }
        public string Header { get; set; }
        public List<FaqWidgetItemViewModel> Items { get; set; }
    }

    public class FaqWidgetItemViewModel
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
