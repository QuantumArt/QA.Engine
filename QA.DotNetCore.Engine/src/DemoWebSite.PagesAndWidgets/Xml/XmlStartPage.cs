using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Xml;

namespace DemoWebSite.PagesAndWidgets.Xml
{
    public class XmlStartPage : XmlAbstractPage, IStartPage
    {
        public string[] GetDNSBindings()
        {
            return GetDetail("HostBinding", new string[1] { "*" });
        }

        //public ITargetingUrlResolver GetUrlResolver()
        //{
        //    return null;
        //}
    }
}
