using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using QA.DotNetCore.Engine.QpData.Interfaces;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Элемент структуры сайта QP
    /// </summary>
    public abstract class AbstractItem : AbstractItemBase, IAbstractItem
    {
        private IAbstractItemContextStorageBuilder _builder;

        public AbstractItem(string discriminator) : this()
        {
            Discriminator = discriminator;
        }
        public AbstractItem()
        {
            Children = new HashSet<IAbstractItem>();
            M2MRelations = new M2MRelations();
            M2MFieldNameMapToLinkIds = new Dictionary<string, int>();
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

        internal AbstractItem CleanVersionOf()
        {
            VersionOf = null;
            return this;
        }

        internal AbstractItem CleanChildren()
        {
            Children = new HashSet<IAbstractItem>();
            return this;
        }

        public IAbstractItem VersionOf { get; set; }
        public ICollection<IAbstractItem> Children { get; set; }
        public int? RawSortOrder { get; set; }
        public int? ExtensionId { get; set; }
        public int? ParentId { get; set; }
        public int? VersionOfId { get; set; }
        public string Discriminator { get; set; }
        public bool Published { get; set; }
        
        public Func<AbstractItemExtensionCollection> GetDetails { get; set; }
        
        public AbstractItemExtensionCollection Details { get; set; }
        public M2MRelations M2MRelations { get; set; }
        public Dictionary<string, int> M2MFieldNameMapToLinkIds { get; set; }

        /// <summary>
        /// Получение свойств расширения
        /// </summary>
        public virtual T GetDetail<T>(string name, T defaultValue)
        {
            VerifyDetailsLoaded();
            
            object value = Details.Get(name, typeof(T));
            if (value == null)
            {
                return defaultValue;
            }
            return (T)value;
        }

        public void SetBuilder(IAbstractItemContextStorageBuilder builder) => _builder = builder;

        public void VerifyDetailsLoaded()
        {
            if (Details == null && _builder != null)
            {
                Details = _builder.BuildDetails(this, true);
            }
        }

        /// <summary>
        /// Получение ссылок m2m
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IEnumerable<int> GetRelationIds(string name)
        {
            if (M2MRelations == null)
            {
                return Enumerable.Empty<int>();
            }

            var linkId = M2MFieldNameMapToLinkIds.TryGetValue(name.ToLowerInvariant(), out int value) ? value : 0 ;
            return M2MRelations.GetRelationValue(linkId);
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
