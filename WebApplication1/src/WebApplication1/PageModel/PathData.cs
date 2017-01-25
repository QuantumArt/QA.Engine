namespace WebApplication1.PageModel
{
    public class PathData
    {
        private AbstractItem abstractItem;
        private string remainingUrl;

        public PathData(AbstractItem abstractItem, string remainingUrl)
        {
            this.abstractItem = abstractItem;
            this.remainingUrl = remainingUrl;
        }

        public AbstractItem AbstractItem
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