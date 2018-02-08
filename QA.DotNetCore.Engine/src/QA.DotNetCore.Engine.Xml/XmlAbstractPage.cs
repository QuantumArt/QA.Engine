using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Xml
{
    public abstract class XmlAbstractPage : XmlAbstractItem, IAbstractPage
    {
        public override bool IsPage => true;

        public bool IsVisible => true;
    }
}
