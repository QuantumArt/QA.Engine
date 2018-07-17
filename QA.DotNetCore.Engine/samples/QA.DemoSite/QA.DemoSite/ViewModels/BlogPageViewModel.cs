using QA.DotNetCore.Engine.QpData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DemoSite.ViewModels
{
    public class BlogPageViewModel
    {
        public BlogPageViewModel()
        {
            Items = new List<BlogItemInListViewModel>();
        }

        public string Header { get; set; }
        public List<BlogItemInListViewModel> Items { get; set; }
    }

    public class BlogItemInListViewModel
    {
        public string Title { get; set; }
        public string Brief { get; set; }
        public string CategoryName { get; set; }
        public string Date { get; set; }

        public string Image { get; set; }
        public string YoutubeVideoCode { get; set; }

        public string Url { get; set; }
    }
}
