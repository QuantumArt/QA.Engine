using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
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

        internal void AddChild(AbstractItem child)
        {
            _children.Add(child);
            child.Parent = this;
            child.ParentId = this.Id;
        }

        public int Id { get; set; }
        public IAbstractItem Parent { get; private set; }
        public int? ParentId { get; private set; }
        public string Alias { get; internal set; }
        public string Title { get; internal set; }
        public abstract bool IsPage { get; }
        internal int? ExtensionId { get; set; }
        internal AbstractItemExtensionCollection Details { get; set; }
        internal AbstractItemM2mRelations M2mRelations { get; set; }

        public string GetUrl()
        {
            return GetTrail();
        }

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
            return filter == null ? _children : _children.Pipe(filter);
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
        /// Получение свойств расширения
        /// </summary>
        public virtual T GetDetail<T>(string name, T defaultValue)
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

        /// <summary>
        /// Получение ссылок m2m
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IEnumerable<int> GetRelationIds(string name)
        {
            if (M2mRelations == null)
                return new int[0];
            var relationId = GetDetail(name, 0);
            return M2mRelations.GetRelationValue(relationId);
        }

        /// <summary>
        /// Получение типа элемента
        /// </summary>
        /// <returns></returns>
        public Type GetContentType()
        {
            return this.GetType();
        }

        public virtual object GetTargetingValue(string targetingKey)
        {
            //для проверки
            if (targetingKey == "culture")
            {
                if (Alias == "proxyabove" || Alias == "philosophy")
                    return "en-us";
            }

            return null;//пока AbstractItem не научили таргетироваться
        }
    }
}
