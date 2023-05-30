using QA.DotNetCore.Engine.Abstractions;
using System;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;

namespace QA.DotNetCore.Engine.QpData
{
    public abstract class AbstractWidget : AbstractItem, IAbstractWidget
    {
        readonly Lazy<string[]> _lazyAllowedUrlPatterns;
        readonly Lazy<string[]> _lazyDeniedUrlPatterns;

        public AbstractWidget() : base()
        {
            _lazyAllowedUrlPatterns = new Lazy<string[]>(() => this.GetAllowedUrlPatterns());
            _lazyDeniedUrlPatterns = new Lazy<string[]>(() => this.GetDeniedUrlPatterns());
        }

        public override bool IsPage
        {
            get
            {
                return false;
            }
        }

        public virtual string ZoneName { get; set; }

        public virtual string[] AllowedUrlPatterns
        {
            get
            {
                return _lazyAllowedUrlPatterns.Value;
            }
        }

        public virtual string[] DeniedUrlPatterns
        {
            get
            {
                return _lazyDeniedUrlPatterns.Value;
            }
        }

        internal override void MapPersistent(AbstractItemPersistentData persistentItem)
        {
            base.MapPersistent(persistentItem);
            ZoneName = persistentItem.ZoneName;
        }
    }
}
