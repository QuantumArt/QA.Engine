using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;

namespace QA.DotNetCore.Engine.QpData
{
    public class UniversalWidget : UniversalAbstractItem, IAbstractWidget
    {
        readonly Lazy<string[]> _lazyAllowedUrlPatterns;
        readonly Lazy<string[]> _lazyDeniedUrlPatterns;

        public UniversalWidget(string discriminator, Definition definition) : base(discriminator)
        {
            IsPage = false;
            Definition = definition;
            _lazyAllowedUrlPatterns = new Lazy<string[]>(() => this.GetAllowedUrlPatterns());
            _lazyDeniedUrlPatterns = new Lazy<string[]>(() => this.GetDeniedUrlPatterns());
        }

        public string ZoneName { get; set; }

        public string[] AllowedUrlPatterns => _lazyAllowedUrlPatterns.Value;

        public string[] DeniedUrlPatterns => _lazyDeniedUrlPatterns.Value;

        internal override void MapPersistent(AbstractItemPersistentData persistentItem)
        {
            base.MapPersistent(persistentItem);
            ZoneName = persistentItem.ZoneName;
        }
    }

}
