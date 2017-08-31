using QA.DotNetCore.Engine.Abstractions;
using System;
using QA.DotNetCore.Engine.QpData.Persistent.Data;

namespace QA.DotNetCore.Engine.QpData
{
    public abstract class AbstractWidget : AbstractItem, IAbstractWidget
    {
        Lazy<string[]> _lazyAllowedUrlPatterns;
        Lazy<string[]> _lazyDeniedUrlPatterns;
        public AbstractWidget() : base()
        {
            _lazyAllowedUrlPatterns = new Lazy<string[]>(() => GetDetail("AllowedUrlPatterns", string.Empty)?.Split(new char[] { '\n', '\r', ';', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries));
            _lazyDeniedUrlPatterns = new Lazy<string[]>(() => GetDetail("DeniedUrlPatterns", string.Empty)?.Split(new char[] { '\n', '\r', ';', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public override bool IsPage
        {
            get
            {
                return false;
            }
        }

        public string ZoneName { get; private set; }

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
