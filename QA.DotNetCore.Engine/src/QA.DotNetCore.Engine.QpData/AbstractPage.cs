using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Persistent.Data;

namespace QA.DotNetCore.Engine.QpData
{
    public abstract class AbstractPage : AbstractItem, IAbstractPage
    {
        public override bool IsPage
        {
            get
            {
                return true;
            }
        }

        public bool IsVisible { get; internal set; }

        internal override void MapPersistent(AbstractItemPersistentData persistentItem)
        {
            base.MapPersistent(persistentItem);
            IsVisible = persistentItem.IsVisible ?? false;
        }
    }
}
