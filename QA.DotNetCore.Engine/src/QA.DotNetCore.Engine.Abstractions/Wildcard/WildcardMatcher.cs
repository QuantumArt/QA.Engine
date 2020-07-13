using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace QA.DotNetCore.Engine.Abstractions.Wildcard
{
    public class WildcardMatcher : IWildcardMatcher
    {
        private readonly Dictionary<string, Regex> _dictionary;
        private readonly WildcardMatchingOption _option;

        public WildcardMatcher(params string[] patterns)
            : this(WildcardMatchingOption.FullMatch, patterns)
        { }

        public WildcardMatcher(WildcardMatchingOption option, params string[] patterns)
            : this(option, (IEnumerable<string>) patterns)
        { }

        public WildcardMatcher(WildcardMatchingOption option, IEnumerable<string> patterns)
        {
            _dictionary = new Dictionary<string, Regex>();
            _option = option;
            foreach (var item in patterns.Distinct())
            {
                _dictionary.Add(item, PrepareExpression(item));
            }
        }


        public virtual IEnumerable<string> Match(string text)
        {
            foreach (var item in _dictionary)
            {
                if (item.Value.IsMatch(text))
                {
                    yield return item.Key;
                }
            }
        }

        /// <summary>
        /// Возвращает наименее общее правило (самый длинный шаблон), которому удовлетворяет строка
        /// </summary>
        /// <param name="text">строка для проверки</param>
        /// <returns></returns>
        public string MatchLongest(string text)
        {
            return Match(text)
                .OrderByDescending(x => x.Length)
                .ThenByDescending(x => x)
                .FirstOrDefault();
        }

        private Regex PrepareExpression(string pattern)
        {
            const string placeholder = "__fake_123_";
            string escaped = "";

            if ((_option & WildcardMatchingOption.StartsWith) != 0)
            {
                escaped = @"^" + escaped;
            }

            escaped +=  Regex
                           .Escape(pattern.Replace("*", placeholder))
                           .Replace(placeholder, @"[\w+-_]*");

            if ((_option & WildcardMatchingOption.EndsWith) != 0)
            {
                escaped += "$";
            }

            var expr = new Regex(escaped, ((_option & WildcardMatchingOption.CaseSensitive) == 0) ?
                RegexOptions.IgnoreCase :
                RegexOptions.None
            );

            return expr;
        }
    }
}
