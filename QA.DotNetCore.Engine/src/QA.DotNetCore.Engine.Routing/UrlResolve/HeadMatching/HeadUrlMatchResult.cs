using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching
{
    public class HeadUrlMatchResult
    {
        public static HeadUrlMatchResult NotMatch { get { return new HeadUrlMatchResult(); } }
        public static HeadUrlMatchResult MatchOnlyForAuthority { get { return new HeadUrlMatchResult { IsMatchForAuthority = true }; } }

        public HeadUrlMatchResult()
        {
            TokenValues = new Dictionary<string, string>();
        }

        public bool IsMatch { get; set; }
        public bool IsMatchForAuthority { get; set; }
        public Dictionary<string, string> TokenValues { get; set; }
        public HeadUrlMatchingPattern Pattern { get; set; }
        public string SanitizedUrl { get; set; }

        /// <summary>
        /// Все ли токены из шаблона обнаружены?
        /// </summary>
        public bool AllTokenFound
        {
            get
            {
                if (Pattern == null)
                    return false;
                return Pattern.Tokens.All(t => TokenValues.ContainsKey(t.Name));
            }
        }
    }
}
