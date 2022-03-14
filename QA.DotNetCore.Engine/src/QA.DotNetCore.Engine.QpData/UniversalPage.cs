using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;

namespace QA.DotNetCore.Engine.QpData
{
    public class UniversalPage : UniversalAbstractItem, IAbstractPage, IStartPage
    {
        public UniversalPage(string discriminator) : base(discriminator)
        {
            IsPage = true;
        }

        public bool IsVisible { get; set; }

        public string[] GetDNSBindings()
        {
            return this.GetBindings();
        }

        internal override void MapPersistent(AbstractItemPersistentData persistentItem)
        {
            base.MapPersistent(persistentItem);
            IsVisible = persistentItem.IsVisible ?? false;
        }
    }
}