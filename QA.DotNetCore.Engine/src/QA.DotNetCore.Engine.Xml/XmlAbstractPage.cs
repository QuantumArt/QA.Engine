using QA.DotNetCore.Engine.Abstractions;
using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Xml
{
    public abstract class XmlAbstractPage : XmlAbstractItem, IAbstractPage
    {
        public override bool IsPage => true;

        public virtual bool IsVisible { get; set; }

        private const string VisibleAttrKey = "IsVisible";

        internal override void Init(IDictionary<string, string> attrs, int id, XmlAbstractItem parent)
        {
            base.Init(attrs, id, parent);
            IsVisible = attrs.ContainsKey(VisibleAttrKey) ? Convert.ToBoolean(attrs[VisibleAttrKey]) : true;
        }
    }
}
