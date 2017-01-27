using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Common.PageModel;

namespace Common.Helpers
{
    public static class HtmlHelpers
    {
        public static HtmlString Tree(this IHtmlHelper html)
        {
            var root = ((AbstractItemStorage)html.ViewContext.HttpContext.RequestServices.GetService(typeof(AbstractItemStorage))).Root;

            var sb = new StringBuilder();

            var node = root;
            sb.Append("<ul>");

            foreach (var item in root.Children)
            {
                VisitNodes(sb, item);
            }

            sb.Append("</ul>");


            return new HtmlString(sb.ToString());
        }

        private static void VisitNodes(StringBuilder sb, AbstractItem node)
        {
            sb.Append($"<li> <a href = {node.GetTrail()}> {node.Title} </a></li>");

            if (node.Children.Count > 0)
            {
                sb.Append("<ul>");
                foreach (var item in node.Children)
                {
                    VisitNodes(sb, item);
                }
                sb.Append("</ul>");
            }

        }
    }
}
