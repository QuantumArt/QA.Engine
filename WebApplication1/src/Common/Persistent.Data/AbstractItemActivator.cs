using Common.PageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Persistent.Data
{
    public class AbstractItemActivator
    {
        public AbstractItem Activate(AbstractItemPersistentData persistent)
        {
            switch (persistent.Discriminator)
            {
                case 231086:
                    return new StartPage(persistent.Id, persistent.Alias, persistent.Title);
                default:
                    return new TextPage(persistent.Id, persistent.Alias, persistent.Title);
            }
        }
    }
}
