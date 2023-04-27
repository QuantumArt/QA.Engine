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
        Definition Definition { get; }
        string Alias { get; }
        string Title { get; }
        bool IsPage { get; }
        int SortOrder { get; }
        string GetUrl(ITargetingUrlTransformator urlResolver = null);
        string GetTrail();
        IEnumerable<IAbstractItem> GetChildren(ITargetingFilter filter = null);
        IEnumerable<TAbstractItem> GetChildren<TAbstractItem>(ITargetingFilter filter = null) where TAbstractItem : class, IAbstractItem;
        IAbstractItem GetChildPageByAlias(string alias, ITargetingFilter filter = null);
        object GetTargetingValue(string targetingKey);
        object GetMetadata(string key);
    }
}
