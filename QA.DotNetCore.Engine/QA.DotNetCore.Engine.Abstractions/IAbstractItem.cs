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
        IEnumerable<IAbstractItem> Children { get; }
        string Alias { get; }
        string Title { get; }
        bool IsPage { get; }

        string GetTrail();
        IAbstractItem Get(string alias);
    }
}
