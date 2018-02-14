using QA.DotNetCore.Engine.Abstractions;
using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Xml
{
    public abstract class XmlAbstractWidget : XmlAbstractItem, IAbstractWidget
    {
        public override bool IsPage => false;

        public virtual string ZoneName { get; private set; }

        public virtual string[] AllowedUrlPatterns => _lazyAllowedUrlPatterns.Value;

        public virtual string[] DeniedUrlPatterns => _lazyDeniedUrlPatterns.Value;

        private const string ZoneAttrKey = "Zone";
        private const string AllowedUrlsAttrKey = "AllowedUrls";
        private const string DeniedUrlsAttrKey = "DeniedUrls";

        Lazy<string[]> _lazyAllowedUrlPatterns;
        Lazy<string[]> _lazyDeniedUrlPatterns;
        public XmlAbstractWidget() : base()
        {
            _lazyAllowedUrlPatterns = new Lazy<string[]>(() => GetDetail(AllowedUrlsAttrKey, new string[0]));
            _lazyDeniedUrlPatterns = new Lazy<string[]>(() => GetDetail(DeniedUrlsAttrKey, new string[0]));
        }

        internal override void Init(IDictionary<string, string> attrs, int id, XmlAbstractItem parent)
        {
            base.Init(attrs, id, parent);
            ZoneName = attrs.ContainsKey(ZoneAttrKey) ? attrs[ZoneAttrKey] : null;
        }
    }
}
