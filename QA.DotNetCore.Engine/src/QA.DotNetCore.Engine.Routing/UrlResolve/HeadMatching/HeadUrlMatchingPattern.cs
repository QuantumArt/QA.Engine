using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching
{
    public class HeadUrlMatchingPattern
    {
        public HeadUrlMatchingPattern()
        {
            Defaults = new Dictionary<string, string>();
        }

        public string Pattern { get; set; }

        public Dictionary<string, string> Defaults { get; set; }

        public UrlMatchingToken[] Tokens
        {
            get
            {
                Url pUrl = Pattern;
                var pSegments = pUrl.GetSegments();

                var tokens = new List<UrlMatchingToken>();
                foreach (var match in Regex.Matches(Pattern, @"\{(.*?)\}"))
                {
                    var tokenName = match.ToString();
                    bool isInAuthority = false;

                    int tokenPosition;
                    if (pUrl.Authority != null && pUrl.Authority.Contains(tokenName))
                    {
                        var domains = pUrl.GetReversedDomains();

                        tokenPosition = Array.IndexOf(domains, tokenName);

                        isInAuthority = true;
                    }
                    else
                    {
                        tokenPosition = Array.IndexOf(pSegments, tokenName);
                    }

                    tokens.Add(new UrlMatchingToken
                    {
                        IsInAuthority = isInAuthority,
                        Position = tokenPosition,
                        Name = tokenName.Substring(1, tokenName.Length - 2) //обрезаем фигурные скобки
                    });
                }
                return tokens.ToArray();
            }
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
}
