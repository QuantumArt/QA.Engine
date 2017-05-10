using Common.PageModel;
using System.Collections.Generic;

namespace Common.Persistent
{
    public interface IAbstractItemRepository
    {
        IEnumerable<AbstractItem> GetAll();
    }
}
