using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.QpData.Persistent.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

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
        }

        public int Id { get; internal set; }
        public IAbstractItem Parent { get; private set; }
        public int? ParentId { get; internal set; }
        public string Alias { get; internal set; }
        public string Title { get; internal set; }
        public int SortOrder { get; internal set; }
        public abstract bool IsPage { get; }
        internal int? ExtensionId { get; set; }
        internal AbstractItemExtensionCollection Details { get; set; }
        internal M2mRelations M2mRelations { get; set; } = new M2mRelations();

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


//#if NETFX_462 || NETFX_47 || NET462 || NET47
//        /// <summary>
//        /// Получение свойств расширения, названия которых совпадают с именем поля.
//        /// </summary>
//        public virtual T GetValue<T>(T defaultValue, [CallerMemberName] string name = "")
//        {
//            return GetDetail(name, defaultValue);
//        }

//#endif

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
            return GetType();
        }

        /// <summary>
        /// Получить значение таргетирования по ключу (ключ определяет систему таргетирования, например, регион, культура итп)
        /// </summary>
        /// <param name="targetingKey"></param>
        /// <returns></returns>
        public virtual object GetTargetingValue(string targetingKey)
        {
            return null; //пока AbstractItem не научили таргетироваться
        }

        /// <summary>
        /// Список id регионов
        /// </summary>
        public virtual IEnumerable<int> RegionIds { get { return GetRelationIds("Regions"); } }

        /// <summary>
        /// Id культуры
        /// </summary>
        public virtual int? CultureId { get { return GetDetail("Culture", default(int?)); } }
    }
}
