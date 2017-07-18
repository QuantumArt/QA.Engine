using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.Targeting;

namespace QA.DotNetCore.Engine.Widgets
{
    public static class ComponentExtensions
    {
        public static async Task<IHtmlContent> WidgetZone(this IViewComponentHelper helper, IHtmlHelper html, string zoneName, object arguments = null)
        {
            var builder = new HtmlContentBuilder();
            var currentPage = html.ViewContext.RouteData.DataTokens["ui-item"] as IAbstractItem;

            if (html.ViewContext.HttpContext.Items.ContainsKey("start-redendering-widgets"))
            {
                currentPage = html.ViewBag.CurrentItem as IAbstractItem;
            }

            var sb = new StringBuilder();
            if (currentPage != null)
            {
                var mapper = ((IComponentMapper)html.ViewContext.HttpContext.RequestServices.GetService(typeof(IComponentMapper)));
                var filter = ((ITargetingFilterAccessor)html.ViewContext.HttpContext.RequestServices.GetService(typeof(ITargetingFilterAccessor)))?.Get();

                var children = currentPage.GetChildren(filter)
                    .OfType<IAbstractWidget>()
                    .Where(x => string.Equals(x.ZoneName, zoneName));

                using (var writer = new StringWriter(sb))
                {
                    builder.AppendHtml($"<!--start zone {zoneName} -->");

                    foreach (var widget in children)
                    {
                        var name = mapper.Map(widget);
                        builder.AppendHtml($"<!-- start render widget {widget.Id} -->");
                        try
                        {
                            html.ViewContext.HttpContext.Items["start-redendering-widgets"] = true;
                            html.ViewContext.HttpContext.Items["should-use-custom-invoker"] = true;
                            html.ViewContext.HttpContext.Items["ui-part"] = widget;
                            var result = await helper.InvokeAsync(name, arguments);
                            builder.AppendHtml(result);
                        }
                        finally
                        {
                            html.ViewContext.HttpContext.Items.Remove("ui-part");
                            html.ViewContext.HttpContext.Items.Remove("start-redendering-widgets");
                            builder.AppendHtml($"<!-- finish render widget {widget.Id} -->");
                        }
                    }

                    builder.AppendHtml($"<!--start zone {zoneName} -->");
                }
            }
            return builder;

        }
    }
}
