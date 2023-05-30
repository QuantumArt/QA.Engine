using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Xml
{
    public abstract class XmlAbstractItem : AbstractItemBase, IAbstractItem
    {
        public XmlAbstractItem()
        {
            Children = new HashSet<IAbstractItem>();
        }

        /// <summary>
        /// Получение дочерних элементов
        /// </summary>
        /// <param name="filter">Опционально. Фильтр таргетирования</param>
        /// <returns></returns>
        public override IEnumerable<IAbstractItem> GetChildren(ITargetingFilter filter = null)
        {
            return filter == null ? Children : Children.Pipe(filter);
        }

        public override IEnumerable<TAbstractItem> GetChildren<TAbstractItem>(ITargetingFilter filter = null)
        {
            return GetChildren(filter).OfType<TAbstractItem>();
        }

        public override object GetMetadata(string key)
        {
            return null;
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

        internal ICollection<IAbstractItem> Children { get; set; }
        internal IDictionary<string, string> Attributes { get; set; }

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
            else if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return Convert.ToDecimal(value);
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
