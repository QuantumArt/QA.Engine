using QA.DotNetCore.Engine.Abstractions;

namespace DemoWebSite.PagesAndWidgets
{
    public class DemoControllerMapper : IControllerMapper
    {
        public string Map(IAbstractItem page)
        {
            var name = page.GetType().Name;
            switch (name)
            {
                case "StartPage": return name;
                case "TextPage": return name;

                default:
                    break;
            }

            return null;
        }
    }
}
