using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Элемент структуры сайта QP
    /// </summary>
    public abstract class AbstractItem : AbstractItemBase, IAbstractItem
    {
        public AbstractItem()
        {
            Children = new HashSet<IAbstractItem>();
            M2mRelations = new M2mRelations();
            M2mFieldNames = new List<string>();
        }

        public override int SortOrder { get => RawSortOrder ?? 0; }

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
            switch (key)
            {
                case OnScreenWidgetMetadataKeys.Type:
                    return Discriminator;
                case OnScreenWidgetMetadataKeys.Published:
                    return Published;
                default:
                    return null;
            }
        }

        internal virtual void AddChild(AbstractItem child)
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
            Discriminator = persistentItem.Discriminator;
            Published = persistentItem.Published;
        }

        internal virtual void MapVersionOf(AbstractItem main)
        {
            VersionOf = main;
            Alias = main.Alias;//у контентной версии не проставлен алиас, берём из основной
            Children = main.Children;//у контентной версии должно быть те же дочерние элементы, что и у основной
            if (!RawSortOrder.HasValue)//если у контентной версии нет порядкового номера, берём его у основной
                RawSortOrder = main.RawSortOrder;
        }

        internal IAbstractItem VersionOf { get; private set; }
        internal ICollection<IAbstractItem> Children { get; set; }
        internal int? RawSortOrder { get; set; }
        internal int? ExtensionId { get; set; }
        internal int? ParentId { get; set; }
        internal int? VersionOfId { get; set; }
        internal string Discriminator { get; set; }
        internal bool Published { get; set; }
        internal AbstractItemExtensionCollection Details { get; set; }
        internal M2mRelations M2mRelations { get; set; }
        internal List<string> M2mFieldNames { get; set; }

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
            return GetType();
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
