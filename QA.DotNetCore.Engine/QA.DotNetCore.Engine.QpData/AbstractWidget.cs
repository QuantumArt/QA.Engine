using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.QpData
{
    public abstract class AbstractWidget : AbstractItem, IAbstractWidget
    {
        public string ZoneName { get; set; }

        public override bool IsPage
        {
            get
            {
                return false;
            }
        }
    }
}
