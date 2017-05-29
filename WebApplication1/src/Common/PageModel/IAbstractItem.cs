using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.PageModel
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
        string Alias { get; set; }
        string Title { get; set; }
        bool IsPage { get; }

        string GetTrail();
        IAbstractItem Get(string alias);
    }
}
