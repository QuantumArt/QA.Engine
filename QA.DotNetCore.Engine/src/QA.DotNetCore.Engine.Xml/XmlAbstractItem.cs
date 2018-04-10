using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QA.DotNetCore.Engine.Xml
{
    public abstract class XmlAbstractItem : IAbstractItem
    {
        public XmlAbstractItem()
        {
            Children = new HashSet<IAbstractItem>();
        }

        private const string AliasAttrKey = "Alias";
        private const string TitleAttrKey = "Title";
        private const string SortOrderAttrKey = "Order";

        internal virtual void Init(IDictionary<string, string> attrs, int id, XmlAbstractItem parent)
        {
            Id = id;
            Parent = parent;
            if (parent != null)
            {
                parent.Children.Add(this);
            }
            Alias = attrs.ContainsKey(AliasAttrKey) ? attrs[AliasAttrKey] : null;
            Title = attrs.ContainsKey(TitleAttrKey) ? attrs[TitleAttrKey] : null;
            var sortOrder = 0;
            if (attrs.ContainsKey(SortOrderAttrKey))
            {
                Int32.TryParse(attrs[SortOrderAttrKey], out sortOrder);
            }
            SortOrder = sortOrder;
            Attributes = attrs;
        }

        internal ICollection<IAbstractItem> Children { get; private set; }
        internal IDictionary<string, string> Attributes { get; private set; }

        public int Id { get; private set; }
        public IAbstractItem Parent { get; private set; }
        public string Alias { get; private set; }
        public string Title { get; private set; }
        public abstract bool IsPage { get; }
        public int SortOrder { get; private set; }

        public string GetUrl()
        {
            return GetTrail();
        }

        private string _url;

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
        /// Получение дочерних элементов
        /// </summary>
        /// <param name="filter">Опционально. Фильтр таргетирования</param>
        /// <returns></returns>
        public IEnumerable<IAbstractItem> GetChildren(ITargetingFilter filter = null)
        {
            return filter == null ? Children : Children.Pipe(filter);
        }

        /// <summary>
        /// Получение дочернего элемента по алиасу
        /// </summary>
        /// <param name="alias">Алиас искомого элемента</param>
        /// <param name="filter">Опционально. Фильтр таргетирования</param>
        /// <returns></returns>
        public IAbstractItem Get(string alias, ITargetingFilter filter = null)
        {
            return GetChildren(filter).FirstOrDefault(x => string.Equals(x.Alias, alias, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Получить значение таргетирования по ключу (ключ определяет систему таргетирования, например, регион, культура итп)
        /// </summary>
        /// <param name="targetingKey"></param>
        /// <returns></returns>
        public virtual object GetTargetingValue(string targetingKey)
        {
            return null;
        }

        /// <summary>
        /// Получение значений из атрибута
        /// </summary>
        public virtual T GetDetail<T>(string name, T defaultValue)
        {
            if (Attributes == null || !Attributes.ContainsKey(name))
            {
                return defaultValue;
            }
            var value = ConvertValue(Attributes[name], typeof(T));
            if (value == null)
            {
                return defaultValue;
            }
            return (T)value;
        }

        private object ConvertValue(string value, Type type)
        {
            if (type == typeof(string))
            {
                return value;
            }
            else if (value == null)
            {
                return null;
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                return Convert.ToDouble(value);
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                return Convert.ToInt32(value);
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                return Convert.ToInt64(value);
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return Convert.ToDateTime(value);
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                return Convert.ToBoolean(value);
            }
            else if (type == typeof(string[]))
            {
                return value.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (type == typeof(int[]))
            {
                return value.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries).Where(s => Int32.TryParse(s, out int tmp)).Select(s => Int32.Parse(s)).ToArray();
            }

            return value;
        }
    }
}
