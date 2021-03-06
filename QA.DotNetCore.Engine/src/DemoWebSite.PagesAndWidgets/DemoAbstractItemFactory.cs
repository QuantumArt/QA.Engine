using DemoWebSite.PagesAndWidgets.Pages;
using DemoWebSite.PagesAndWidgets.Widgets;
using QA.DotNetCore.Engine.QpData;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.Routing.UrlResolve;
using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;

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
                case "blog_page":
                    return new BlogPage();
                case "html_part":
                    return new TextPart();
                case "news_widget":
                    return new TextPart();
                case "full-width-container":
                    return new TextPart();
                case "paralax-container":
                    return new TextPart();
                case "blog_widget":
                    return new TextPart();
                case "simple-gallery":
                    return new TextPart();
                default:
                    return null;

            }
        }
    }
}
