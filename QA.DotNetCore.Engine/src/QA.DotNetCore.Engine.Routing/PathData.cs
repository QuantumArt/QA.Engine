using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Routing
{
    public class PathData
    {
        private readonly IAbstractItem abstractItem;
        private readonly string remainingUrl;

        public PathData(IAbstractItem abstractItem, string remainingUrl)
        {
            this.abstractItem = abstractItem;
            this.remainingUrl = remainingUrl;
        }

        public IAbstractItem AbstractItem
        {
            get
            {
                return abstractItem;
            }

        }

        public string RemainingUrl
        {
            get
            {
                return remainingUrl;
            }
        }
    }
}
