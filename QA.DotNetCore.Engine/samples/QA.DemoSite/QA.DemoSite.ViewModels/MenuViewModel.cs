using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DemoSite.ViewModels
{
    public class MenuViewModel
    {
        public List<MenuItem> Items { get; set; } = new List<MenuItem>();
    }

    public class MenuItem
    {
        public string Title { get; set; }
        public string Alias { get; set; }
        public string Href { get; set; }
        public List<MenuItem> Children { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public bool HasActiveChild { get; set; }

        public MenuItem()
        {
            Children = new List<MenuItem>();
        }
    }
}
