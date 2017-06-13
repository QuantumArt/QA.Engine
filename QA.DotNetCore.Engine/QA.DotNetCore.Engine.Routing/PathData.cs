using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Routing
{
    internal class PathData
    {
        private IAbstractItem abstractItem;
        private string remainingUrl;

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
