using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Routing
{
    public class UrlTokenConfig
    {
        public List<UrlMatchingPattern> MatchingPatterns { get; set; }
    }

    public class UrlMatchingPattern
    {
        public string Value { get; set; }

        public UrlMatchingToken[] Tokens { get; set; }

        public KeyValuePair<string, string>[] Defaults { get; set; }

        //public int CultureTokenPosition;
        //public int RegionTokenPosition;
        //public bool ProcessCultureTokens { get; set; }
        //public bool ProcessRegionTokens { get; set; }
        //public string DefaultCultureToken { get; set; }
        //public bool IsRegionInAuthority { get; set; }
        //public bool UseForReplacing { get; set; }
        //public bool IsCultureInAuthority { get; set; }
    }

    public class UrlMatchingToken
    {
        /// <summary>
        /// Название токена
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Признак того, что токен находится в домене. Например http://{region}.example.com/
        /// </summary>
        public bool IsInAuthority { get; set; }

        /// <summary>
        /// Позиция токена в паттерне урла
        /// </summary>
        public int Position { get; set; }
    }
}
