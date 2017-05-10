using Common.Persistent.State;
using System.Collections.Generic;

namespace Common.Persistent
{
    public interface IAbstractItemRepository
    {
        IEnumerable<AbstractItemState> GetAll();
    }
}
