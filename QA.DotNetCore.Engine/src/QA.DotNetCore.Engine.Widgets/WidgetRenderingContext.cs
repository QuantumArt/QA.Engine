using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Widgets
{
    public class WidgetRenderingContext
    {
        public IAbstractItem CurrentWidget { get; set; }

        public bool ShouldUseCustomInvoker { get; set; }
    }
}
