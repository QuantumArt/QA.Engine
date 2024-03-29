using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;

namespace QA.DotNetCore.Engine.QpData
{
    public abstract class AbstractPage : AbstractItem, IAbstractPage
    {
        public override bool IsPage => true;

        public bool IsVisible { get; set; }

        internal override void MapPersistent(AbstractItemPersistentData persistentItem)
        {
            base.MapPersistent(persistentItem);
            IsVisible = persistentItem.IsVisible ?? false;
        }
    }
}
