using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using QA.DotNetCore.Engine.Abstractions;

namespace DemoWebSite.PagesAndWidgets.Helpers
{
    public static class HtmlHelpers
    {
        public static HtmlString Tree(this IHtmlHelper html)
        {
            var root = html.ViewContext.HttpContext.Items["start-page"] as IAbstractItem;
            var filter = ((ITargetingFilterAccessor)html.ViewContext.HttpContext.RequestServices.GetService(typeof(ITargetingFilterAccessor))).Get();

            var sb = new StringBuilder();

            var node = root;
            sb.Append("<ul>");

            foreach (var item in root.Children.Pipe(filter))
            {
                VisitNodes(sb, item, filter);
            }

            sb.Append("</ul>");


            return new HtmlString(sb.ToString());
        }

        private static void VisitNodes(StringBuilder sb, IAbstractItem node, ITargetingFilter filter)
        {
            if (node.IsPage)
                sb.Append($"<li> <a href = {node.GetTrail()}> {node.Title} </a></li>"); 
            else
                sb.Append($"<li> {node.Title} </li>");

            var children = node.Children.Pipe(filter);
            if (children.Any())
            {
                sb.Append("<ul>");
                foreach (var item in children)
                {
                    VisitNodes(sb, item, filter);
                }
                sb.Append("</ul>");
            }

        }
    }
}
