using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Persistent.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Элемент структуры сайта QP
    /// </summary>
    public abstract class AbstractItem : IAbstractItem
    {
        private string _url;
        private IList<IAbstractItem> _children;

        public AbstractItem()
        {
            _children = new List<IAbstractItem>();
        }

        public void AddChild(AbstractItem child)
        {
            _children.Add(child);
            child.Parent = this;
            child.ParentId = this.Id;
        }

        public int Id { get; set; }
        public IAbstractItem Parent { get; private set; }
        public int? ParentId { get; private set; }
        public IEnumerable<IAbstractItem> Children { get { return _children.AsEnumerable(); } }
        public string Alias { get; set; }
        public string Title { get; set; }
        public abstract bool IsPage { get; }
        public int? ExtensionId { get; set; }
        public AbstractItemExtensionCollection Details { get; set; }

        public string GetTrail()
        {
            if (!IsPage)
            {
                return string.Empty;
            }

            if (_url == null)
            {
                var sb = new StringBuilder();
                var item = (this as IAbstractItem);
                while (item != null && !(item is IStartPage))
                {
                    sb.Insert(0, item.Alias + (Parent == null ? "" : "/"));
                    item = item.Parent;
                }

                return (_url = $"/{sb.ToString().TrimEnd('/')}");
            }

            return _url;
        }

        /// <summary>
        /// Получение дочернего элемента по алиасу
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public IAbstractItem Get(string alias)
        {
            return Children.FirstOrDefault(x => string.Equals(x.Alias, alias, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Получение свойств расширения
        /// </summary>
        public T GetDetail<T>(string name, T defaultValue)
        {
            if (Details == null)
            {
                return defaultValue;
            }
            var value = Details.Get(name, typeof(T));
            if (value == null)
            {
                return defaultValue;
            }
            return (T)value;
        }
    }
}
