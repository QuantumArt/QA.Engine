using QA.DotNetCore.Engine.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoWebSite.PagesAndWidgets.Xml
{
    public class XmlTextPage : XmlAbstractPage
    {
        public string Text
        {
            get { return GetDetail("Text", String.Empty); }
        }
    }
}
