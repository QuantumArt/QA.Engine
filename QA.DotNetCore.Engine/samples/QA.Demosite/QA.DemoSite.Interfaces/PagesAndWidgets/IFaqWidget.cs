using QA.DotNetCore.Engine.Abstractions;
using System.Collections.Generic;

namespace QA.DemoSite.Interfaces.PagesAndWidgets
{
    public interface IFaqWidget : IAbstractWidget
    {
        string Header { get; }
        IEnumerable<int> Questions { get; }
    }
}
