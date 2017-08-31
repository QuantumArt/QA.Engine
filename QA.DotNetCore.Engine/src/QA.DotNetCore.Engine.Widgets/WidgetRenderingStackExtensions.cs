using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Widgets
{
    /// <summary>
    /// Расширения над HttpContext для работы со стеком контекста рендеринга виджетов
    /// </summary>
    internal static class WidgetRenderingStackExtensions
    {
        public const string WidgetStackKey = "widgets-stack";

        /// <summary>
        /// Получить текущий контекст рендеринга виджета структуры сайта
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static WidgetRenderingContext GetCurrentRenderingWidgetContext(this HttpContext context)
        {
            var renderingStack = context.Items[WidgetStackKey] as Stack<WidgetRenderingContext>;
            if (renderingStack == null || !renderingStack.Any())
                return null;
            return renderingStack.Peek();
        }

        /// <summary>
        /// Добавить в стек рендеринга виджетов структуры сайта новый виджет
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="newContext"></param>
        /// <returns></returns>
        public static Stack<WidgetRenderingContext> PushWidgetToRenderingStack(this HttpContext httpContext, WidgetRenderingContext newContext)
        {
            var renderingStack = httpContext.Items[WidgetStackKey] as Stack<WidgetRenderingContext>;
            if (renderingStack == null)
            {
                renderingStack = new Stack<WidgetRenderingContext>();
                httpContext.Items[WidgetStackKey] = renderingStack;
            }
            renderingStack.Push(newContext);

            return renderingStack;
        }
    }
}
