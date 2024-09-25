using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using QA.DotNetCore.Engine.Routing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Widgets
{
    public static class ComponentExtensions
    {
        public static async Task<IHtmlContent> WidgetZone(this IViewComponentHelper helper, IHtmlHelper html,
            string zoneName, IDictionary<string, object> arguments = null)
        {
            IEnumerable<IAbstractItem> widgets = null;
            if (ZoneIsGlobal(zoneName))
            {
                //зона глобальная - значит нужны дочерние виджеты стартовой страницы
                var startPage = html.ViewContext.HttpContext.GetStartPage();
                if (startPage != null)
                {
                    widgets = startPage.GetChildren(GetFilter(html, zoneName));
                }
            }
            else
            {
                //элемент структуры сайта внутри которого мы пытаемся отрендерить зону
                IAbstractItem renderingContainer = null;

                var renderingContext = html.ViewContext.HttpContext.GetCurrentRenderingWidgetContext();
                if (renderingContext != null)
                {
                    //рендеримся внутри виджета
                    renderingContainer = renderingContext.CurrentWidget;
                }
                else
                {
                    //рендеримся на странице
                    renderingContainer = html.ViewContext.GetCurrentItem();
                }

                if (renderingContainer != null)
                {
                    if (ZoneIsRecursive(zoneName))
                    {
                        //зона рекурсивная, значит будем искать виджеты с этой зоной не только у текущего элементы, но и у всех родителей
                        var allParents = new List<IAbstractItem>();
                        var current = renderingContainer;
                        while (current.Parent != null)
                        {
                            allParents.Add(current);
                            current = current.Parent;
                        }

                        widgets = allParents.SelectMany(x => x.GetChildren(GetFilter(html, zoneName)));
                    }
                    else
                    {
                        //ищем виджеты с зоной у текущего элемента
                        widgets = renderingContainer.GetChildren(GetFilter(html, zoneName));
                    }
                }
            }

            var builder = new HtmlContentBuilder();

            var onScreenContext = ((IOnScreenContextProvider)html.ViewContext.HttpContext.RequestServices.GetService(typeof(IOnScreenContextProvider)))?.GetContext();
            var isWidgetEditMode = onScreenContext != null ? onScreenContext.HasFeature(OnScreenFeatures.Widgets) : false;
        
            RenderOnScreenModeZoneWrapperStart(isWidgetEditMode, zoneName, builder);
            if (widgets != null)
            {
                var mapper = ((IComponentMapper)html.ViewContext.HttpContext.RequestServices.GetService(typeof(IComponentMapper)));

                foreach (var widget in widgets.OfType<IAbstractWidget>().OrderBy(x => x.SortOrder))
                {
                    var name = mapper.Map(widget);
                    var renderFullWidgetInfo = isWidgetEditMode && !WidgetShouldBeSkipped(onScreenContext, widget);

                    RenderOnScreenModeWidgetWrapperStart(renderFullWidgetInfo, builder, widget);
                    var renderingStack = html.ViewContext.HttpContext.PushWidgetToRenderingStack(new WidgetRenderingContext { CurrentWidget = widget, ShouldUseCustomInvoker = true });
                    try
                    {
                        var result = await helper.InvokeAsync(name, arguments);
                        builder.AppendHtml(result);
                    }
                    finally
                    {
                        renderingStack.Pop();

                        RenderOnScreenModeWidgetWrapperEnd(renderFullWidgetInfo, builder, widget);
                    }
                }
            }
            RenderOnScreenModeZoneWrapperEnd(isWidgetEditMode, zoneName, builder);
            return builder;
        }

        private static bool WidgetShouldBeSkipped(OnScreenContext onScreenContext, IAbstractWidget widget)
        {
            var widgetType = widget.GetMetadata(OnScreenWidgetMetadataKeys.Type).ToString();
            var skipWidgetTypes = onScreenContext?.SkipWidgetTypes;
            return skipWidgetTypes == null ? false : skipWidgetTypes.Contains(widgetType);
        }


        /// <summary>
        /// Рендеринг текста с зонами, объявленных в контенте в виде [[zone=имя_зоны]]
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="componentHelper"></param>
        /// <param name="text"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static async Task<IHtmlContent> RenderZonesInText(this IViewComponentHelper componentHelper,
            IHtmlHelper helper, string text, IDictionary<string, object> arguments = null)
        {
            if (!zonesInTextRegex.IsMatch(text))
            {
                return new HtmlString(text);
            }

            var matches = zonesInTextRegex.Matches(text);
            //наполним словарь для замены зон в тексте на их содержимое
            var dictionary = new Dictionary<string, string>();
            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    var group = match.Groups[1];
                    if (group.Success)
                    {
                        var zoneName = group.Value;

                        if (!string.IsNullOrEmpty(zoneName) && !dictionary.ContainsKey(match.Value))
                        {
                            var widgetContent = await componentHelper.WidgetZone(helper, zoneName, arguments);
                            using (var sw = new StringWriter())
                            {
                                widgetContent.WriteTo(sw, HtmlEncoder.Default);
                                dictionary[match.Value] = sw.ToString();
                            }
                        }
                    }
                }
            }

            //делаем замены
            var result = zonesInTextRegex.Replace(text, (match =>
            {
                if (match.Groups.Count > 1)
                {
                    var group = match.Groups[1];
                    if (group.Success)
                    {
                        return dictionary[match.Value];
                    }
                }
                return string.Empty;
            }));

            return new HtmlString(result);
        }

        private static ITargetingFilter GetFilter(IHtmlHelper html, string zone)
        {
            //фильтр виджетов. проверяет зону и доступен ли для текущего урла
            var widgetFilter = new WidgetFilter(zone, html.ViewContext.HttpContext.Request.Path);
            //общий фильтр структуры сайта
            var siteStructureFilter = ((ITargetingFilterAccessor)html.ViewContext.HttpContext.RequestServices.GetService(typeof(ITargetingFilterAccessor)))?.Get();
            //объединяем их (сначала widgetFilter - как, с большой долей вероятности, более легковесный - порядок имеет значение)
            return siteStructureFilter == null
                ? widgetFilter
                : new UnitedFilter(widgetFilter, siteStructureFilter);
        }

        private static bool ZoneIsRecursive(string zoneName)
        {
            return WidgetZoneTypeQualifier.QualifyZone(zoneName) == WidgetZoneType.Recursive;
        }

        private static bool ZoneIsGlobal(string zoneName)
        {
            return WidgetZoneTypeQualifier.QualifyZone(zoneName) == WidgetZoneType.Global;
        }

        private static void RenderOnScreenModeWidgetWrapperStart(bool renderFullInfo, IHtmlContentBuilder builder, IAbstractItem widget)
        {
            if (renderFullInfo)
                builder.AppendHtml($"<!--start widget {widget.Id} {{ alias='{widget.Alias}' title='{widget.Title.Replace("'", "").Replace("}", "").Replace("{", "")}' type='{widget.GetMetadata(OnScreenWidgetMetadataKeys.Type)}' published='{widget.GetMetadata(OnScreenWidgetMetadataKeys.Published)?.ToString()?.ToLower()}' order='{widget.SortOrder}' }}-->");
            else
                builder.AppendHtml($"<!--start widget-->");
        }

        private static void RenderOnScreenModeZoneWrapperStart(bool isWidgetEditMode, string zoneName, IHtmlContentBuilder builder)
        {
            if (isWidgetEditMode)
                builder.AppendHtml($"<!--start zone {zoneName} {{ isRecursive='{ZoneIsRecursive(zoneName).ToString().ToLowerInvariant()}' isGlobal='{ZoneIsGlobal(zoneName).ToString().ToLowerInvariant()}' }}-->");
            else
                builder.AppendHtml($"<!--start zone {zoneName}-->");
        }

        private static void RenderOnScreenModeWidgetWrapperEnd(bool renderWidgetId, IHtmlContentBuilder builder, IAbstractItem widget)
        {
            if (renderWidgetId)
                builder.AppendHtml($"<!--end widget {widget.Id}-->");
            else
                builder.AppendHtml($"<!--end widget-->");
        }

        private static void RenderOnScreenModeZoneWrapperEnd(bool isWidgetEditMode, string zoneName, IHtmlContentBuilder builder)
        {
            builder.AppendHtml($"<!--end zone {zoneName}-->");
        }

        /// <summary>
        /// Шаблон для замены зон в тексте
        /// Пример: [[zone=имязоны]]
        /// </summary>
        static string ZonePattern = @"\[\[zone=(\w+)\]\]";

        static Regex zonesInTextRegex = new Regex(ZonePattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
    }
}
