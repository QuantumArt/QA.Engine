using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.PageModel
{
    public abstract class AbstractItem
    {
        private string _url;
        public AbstractItem()
        {
            Children = new List<AbstractItem>();
        }

        public AbstractItem(int id, string alias, string title, params AbstractItem[] children)
        {
            Id = id;
            Alias = alias;
            Title = title; Children = new List<AbstractItem>(children);
            foreach (var item in Children)
            {
                item.Parent = this;
            }
            
        }

        public int Id { get; set; }
        public AbstractItem Parent { get; set; }
        public IList<AbstractItem> Children { get; private set; }
        public string Alias { get; set; }
        public string Title { get; set; }

        public string GetTrail()
        {
            if (_url == null)
            {
                var sb = new StringBuilder();
                var item = this;
                while (item != null && !(item is IStartPage))
                {
                    sb.Insert(0, item.Alias + (Parent == null ? "" : "/"));
                    item = item.Parent;
                }

                return (_url = $"/{sb.ToString().TrimEnd('/')}");
            }

            return _url;
        }

        public AbstractItem Get(string alias)
        {
            return Children.FirstOrDefault(x => string.Equals(x.Alias, alias, StringComparison.OrdinalIgnoreCase));
        }
    }
}
