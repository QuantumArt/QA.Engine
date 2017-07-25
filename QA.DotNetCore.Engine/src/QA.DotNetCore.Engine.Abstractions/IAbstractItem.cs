using QA.DotNetCore.Engine.Abstractions.Targeting;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions
{
    /// <summary>
    /// Элемент структуры сайта
    /// </summary>
    public interface IAbstractItem
    {
        int Id { get; }
        IAbstractItem Parent { get; }
        int? ParentId { get; }
        string Alias { get; }
        string Title { get; }
        bool IsPage { get; }
        int SortOrder { get; }
        /// <summary>
        /// Is the item is visible (in navigation, on page, etc)
        /// </summary>
        bool IsVisible { get; }
        string GetUrl();
        string GetTrail();
        IEnumerable<IAbstractItem> GetChildren(ITargetingFilter filter = null);
        IAbstractItem Get(string alias, ITargetingFilter filter = null);
        object GetTargetingValue(string targetingKey);
    }
}
