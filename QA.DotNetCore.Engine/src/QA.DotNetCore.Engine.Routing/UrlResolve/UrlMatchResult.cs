using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Routing.UrlResolve
{
    public class UrlMatchingResult
    {
        public static UrlMatchingResult NotMatch { get { return new UrlMatchingResult(); } }
        public static UrlMatchingResult MatchOnlyForAuthority { get { return new UrlMatchingResult { IsMatchForAuthority = true }; } }

        public UrlMatchingResult()
        {
            TokenValues = new Dictionary<string, string>();
        }

        public bool IsMatch { get; set; }
        public bool IsMatchForAuthority { get; set; }
        public Dictionary<string, string> TokenValues { get; set; }
        public UrlMatchingPattern Pattern { get; set; }
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
