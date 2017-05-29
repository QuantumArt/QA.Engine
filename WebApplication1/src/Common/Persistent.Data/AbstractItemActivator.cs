using Common.PageModel;
using Common.Widgets;
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
                case "root_page":
                    return MapDefaultFields(persistent, new RootPage());
                case "start_page":
                    return MapDefaultFields(persistent, new StartPage());
                case "html_page":
                    return MapDefaultFields(persistent, new TextPage());
                case "html_part":
                    return MapDefaultFields(persistent, new TextPart() { ZoneName = persistent.ZoneName, Text = "Виджет " + persistent.Title });
                default:
                    return null;
                        
            }
        }

        private AbstractItem MapDefaultFields(AbstractItemPersistentData persistent, AbstractItem item)
        {
            item.Id = persistent.Id;
            item.Alias = persistent.Alias;
            item.Title = persistent.Title;
            item.ExtensionId = persistent.ExtensionId;
            return item;
        }
    }
}
