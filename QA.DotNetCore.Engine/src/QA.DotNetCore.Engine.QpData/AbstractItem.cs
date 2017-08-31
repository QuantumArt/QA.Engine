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
        public AbstractItem()
        {
            Children = new HashSet<IAbstractItem>();
            M2mRelations = new M2mRelations();
        }

        internal void AddChild(AbstractItem child)
        {
            Children.Add(child);
            child.Parent = this;
        }

        internal virtual void MapPersistent(AbstractItemPersistentData persistentItem)
        {
            Id = persistentItem.Id;
            Alias = persistentItem.Alias;
            Title = persistentItem.Title;
            RawSortOrder = persistentItem.IndexOrder;
            ExtensionId = persistentItem.ExtensionId;
            ParentId = persistentItem.ParentId;
            VersionOfId = persistentItem.VersionOfId;
        }

        internal virtual void MapVersionOf(AbstractItem main)
        {
            VersionOf = main;
            Alias = main.Alias;//у контентной версии не проставлен алиас, берём из основной
            Children = main.Children;//у контентной версии должно быть те же дочерние элементы, что и у основной
            if (!RawSortOrder.HasValue)//если у контентной версии нет порядкового номера, берём его у основной
                RawSortOrder = main.RawSortOrder;
        }

        public int Id { get; private set; }
        public IAbstractItem Parent { get; private set; }
        public string Alias { get; private set; }
        public string Title { get; private set; }
        public int SortOrder { get { return RawSortOrder ?? 0; } }
        public abstract bool IsPage { get; }
        public IAbstractItem VersionOf { get; private set; }

        internal ICollection<IAbstractItem> Children { get; set; }
        internal int? RawSortOrder { get; set; }
        internal int? ExtensionId { get; set; }
        internal int? ParentId { get; set; }
        internal int? VersionOfId { get; set; }
        internal AbstractItemExtensionCollection Details { get; set; }
        internal M2mRelations M2mRelations { get; set; }

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
