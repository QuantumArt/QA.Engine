using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Routing
{
    public class UrlMatchingResult
    {
        public static UrlMatchingResult Empty { get { return new UrlMatchingResult(); } }

        public UrlMatchingResult()
        {
            TokenValues = new Dictionary<string, string>();
        }

        public bool IsMatch { get; set; }
        public Dictionary<string, string> TokenValues { get; set; }
        public UrlMatchingPattern Pattern { get; set; }
        public string SanitizedUrl { get; set; }
    }
}
