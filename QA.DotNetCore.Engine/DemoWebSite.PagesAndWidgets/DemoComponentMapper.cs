using QA.DotNetCore.Engine.Abstractions;

namespace DemoWebSite.PagesAndWidgets
{
    public class DemoComponentMapper : IComponentMapper
    {
        public string Map(IAbstractItem widget)
        {
            var name = widget.GetType().Name;
            switch (name)
            {
                case "TextPart": return name;

                default:
                    break;
            }

            return null;
        }
    }
}
