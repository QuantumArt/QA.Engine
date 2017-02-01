using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Common.PageModel;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Common.Widgets
{
    public static class ComponentExtensions
    {


        public static async Task<IHtmlContent> WidgetZone(this IViewComponentHelper helper, IHtmlHelper html, string zoneName)
        {
            var builder = new HtmlContentBuilder();
            //var root = ((AbstractItemStorage)html.ViewContext.HttpContext.RequestServices.GetService(typeof(AbstractItemStorage))).Root;
            var currentPage = html.ViewContext.RouteData.DataTokens["ui-item"] as AbstractItem;

            if (html.ViewContext.HttpContext.Items.ContainsKey("start-redendering-widgets"))
            {
                currentPage = html.ViewBag.CurrentItem as AbstractItem;
            }

            var sb = new StringBuilder();
            if (currentPage != null)
            {
                var mapper = new ComponentsMapper();
                var children = currentPage.Children.OfType<AbstractWidget>()
                    .Where(x => string.Equals(x.ZoneName, zoneName) && x is AbstractWidget);

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
                            html.ViewContext.HttpContext.Items["ui-part"] = widget;
                            var result = await helper.InvokeAsync(name, widget);
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
