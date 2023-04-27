using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QA.DotNetCore.Engine.Abstractions
{
    /// <summary>
    /// Элемент структуры сайта (с реализацией базовой логики такой как вычисление trail, получение дочерних элементов итд)
    /// </summary>
    public abstract class AbstractItemBase : IAbstractItem
    {
        private string _trail;

        public virtual int Id { get; protected set; }
        public virtual IAbstractItem Parent { get; protected set; }
        public virtual ItemDefinitionDetails DefinitionDetails { get; protected set; }
        public virtual string Alias { get; protected set; }
        public virtual string Title { get; protected set; }
        public virtual bool IsPage { get; protected set; }
        public virtual int SortOrder { get; protected set; }

        /// <summary>
        /// Получение дочернего элемента (только IsPage) по алиасу
        /// </summary>
        /// <param name="alias">Алиас искомого элемента</param>
        /// <param name="filter">Опционально. Фильтр таргетирования</param>
        /// <returns></returns>
        public virtual IAbstractItem GetChildPageByAlias(string alias, ITargetingFilter filter = null)
        {
            return GetChildren(filter)
                .FirstOrDefault(children => children.IsPage && string.Equals(children.Alias, alias, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Получение дочерних элементов
        /// </summary>
        /// <param name="filter">Опционально. Фильтр таргетирования</param>
        /// <returns></returns>
        public abstract IEnumerable<IAbstractItem> GetChildren(ITargetingFilter filter = null);

        /// <summary>
        /// Получение дочерних элементов
        /// </summary>
        /// <param name="filter">Опционально. Фильтр таргетирования</param>
        /// <returns></returns>
        public abstract IEnumerable<TAbstractItem> GetChildren<TAbstractItem>(ITargetingFilter filter = null) where TAbstractItem : class, IAbstractItem;

        public abstract object GetMetadata(string key);

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
        /// Получить trail элемента, т.е. путь от стартовой страницы до элемента в дереве структуры сайта {alias1}/{alias2}/.../{alias}
        /// </summary>
        /// <returns></returns>
        public string GetTrail()
        {
            if (!IsPage)
            {
                return string.Empty;
            }

            if (_trail == null)
            {
                var sb = new StringBuilder();
                var item = (this as IAbstractItem);
                while (item != null && !(item is IStartPage))
                {
                    sb.Insert(0, item.Alias + (Parent == null ? "" : "/"));
                    item = item.Parent;
                }

                return (_trail = $"/{sb.ToString().TrimEnd('/')}");
            }

            return _trail;
        }

        /// <summary>
        /// Получить url страницы с подставленными значениями таргетирования
        /// </summary>
        /// <returns></returns>
        public string GetUrl(ITargetingUrlTransformator urlTransformator = null)
        {
            var resultUrl = GetTrail();

            if (urlTransformator != null)
            {
                resultUrl = urlTransformator.AddCurrentTargetingValuesToUrl(resultUrl);
            }

            return resultUrl;
        }
    }
}
