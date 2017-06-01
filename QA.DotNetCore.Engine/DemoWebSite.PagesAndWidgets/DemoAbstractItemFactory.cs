using DemoWebSite.PagesAndWidgets.Pages;
using DemoWebSite.PagesAndWidgets.Widgets;
using QA.DotNetCore.Engine.QpData;

namespace DemoWebSite.PagesAndWidgets
{
    public class DemoAbstractItemFactory : IAbstractItemFactory
    {
        public AbstractItem Create(string discriminator)
        {
            switch (discriminator)
            {
                case "root_page":
                    return new RootPage();
                case "start_page":
                    return new StartPage();
                case "html_page":
                    return new TextPage();
                case "html_part":
                    return new TextPart();
                default:
                    return null;

            }
        }
    }
}
