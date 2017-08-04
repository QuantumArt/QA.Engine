using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.QpData
{
    public abstract class AbstractPage : AbstractItem, IAbstractPage
    {
        public override bool IsPage
        {
            get
            {
                return true;
            }
        }

        public bool IsVisible { get; internal set; }
    }
}
