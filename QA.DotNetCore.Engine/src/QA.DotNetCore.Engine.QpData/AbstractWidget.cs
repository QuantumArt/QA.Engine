using QA.DotNetCore.Engine.Abstractions;
using System;

namespace QA.DotNetCore.Engine.QpData
{
    public abstract class AbstractWidget : AbstractItem, IAbstractWidget
    {
        public override bool IsPage
        {
            get
            {
                return false;
            }
        }

        public string ZoneName { get; internal set; }

        public virtual string[] AllowedUrlPatterns
        {
            get
            { 
                return GetDetail("AllowedUrlPatterns", string.Empty)?.Split(new char[] { '\n', '\r', ';', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public virtual string[] DeniedUrlPatterns
        {
            get
            {
                return GetDetail("DeniedUrlPatterns", string.Empty)?.Split(new char[] { '\n', '\r', ';', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
    }
}
