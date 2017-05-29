using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.PageModel
{
    public interface IAbstractItemStorageProvider
    {
        AbstractItemStorage Get(int? rootPageId = null);
    }
}
