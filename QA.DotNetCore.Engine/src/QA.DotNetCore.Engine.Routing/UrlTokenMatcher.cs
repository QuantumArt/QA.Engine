using Microsoft.Extensions.Logging;
using QA.DotNetCore.Engine.Abstractions.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Routing
{
    public class UrlTokenMatcher
    {
        private readonly UrlTokenConfig _config;

        public UrlTokenMatcher(UrlTokenConfig config)
        {
            _config = config;

            foreach (var pattern in _config.MatchingPatterns)
            {
                if (pattern.Defaults == null)
                    pattern.Defaults = new KeyValuePair<string, string>[0];

                Url pUrl = pattern.Value;
                var pSegments = pUrl.GetSegments();

                var tokens = new List<UrlMatchingToken>();
                foreach (var match in Regex.Matches(pattern.Value, @"\{(.*?)\}"))
                {
                    var tokenName = match.ToString();

                    int tokenPosition = -1;
                    bool isInAuthority = false;

                    if (pUrl.Authority.Contains(tokenName))
                    {
                        var domains = GetReversedDomains(pUrl);

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
                pattern.Tokens = tokens.ToArray();
            }
            
        }

        public UrlMatchingResult Match(string originalUrl, ITargetingContext targetingContext)
        {
            Url pUrl = originalUrl;

            var pSegments = pUrl.GetSegments();

            string[] domains = null;

            foreach (var pattern in _config.MatchingPatterns)
            {
                UrlMatchingResult result = new UrlMatchingResult();
                Url sanitized = pUrl;

                if (domains == null && pattern.Tokens.Any(t => t.IsInAuthority) /*pattern.IsRegionInAuthority || pattern.IsCultureInAuthority*/)
                {
                    domains = pUrl.Authority.Split('.')
                        .Select(w => w.Trim())
                        .Where(w => !String.IsNullOrEmpty(w))
                        .Reverse()
                        .ToArray();
                }

                result.IsMatch = true;

                foreach (var token in pattern.Tokens)
                {
                    string r = null;

                    //считаем, что название токена совпадает с ключом системы таргетирования
                    //по этому названию узнаем список возможных значений для токена (если список пуст - считаем что значение может быть любым)
                    var possibleValues = targetingContext.GetPossibleValues(token.Name).Cast<string>().ToArray();

                    if (token.IsInAuthority)
                    {
                        var success = MatchToken(domains, token.Position, possibleValues, out r);
                        if (!success)
                        {
                            result.IsMatch = false;
                            break;
                        }
                    }
                    else
                    {
                        var success = MatchToken(pSegments, token.Position, possibleValues, out r);
                        if (!success)
                        {
                            result.IsMatch = false;
                            break;
                        }
                        if (r != null)
                        {
                            sanitized = sanitized.RemoveSegment(token.Position);
                        }
                    }
                    if (!string.IsNullOrEmpty(r))
                    {
                        result.TokenValues[token.Name] = r;
                    }
                }

                if (!result.IsMatch)
                    continue;

                foreach (var kvp in pattern.Defaults)
                {
                    if (!result.TokenValues.ContainsKey(kvp.Key))
                        result.TokenValues[kvp.Key] = kvp.Value;
                }

                result.Pattern = pattern;
                result.SanitizedUrl = sanitized;
                return result;
            }

            return UrlMatchingResult.Empty;
        }

        public string ReplaceTokens(string originalUrl, Dictionary<string, string> tokenValues, ITargetingContext targetingContext)
        {
            Url url = originalUrl;

            if (!_config.MatchingPatterns.Any())
            {
                return url;
            }

            //проверим, что все новые значения токенов являются допустимыми
            foreach (var t in tokenValues.Keys)
            {
                //считаем, что название токена совпадает с ключом системы таргетирования
                //по этому названию узнаем список возможных значений для токена (если список пуст - считаем что значение может быть любым)
                var possibleValues = targetingContext.GetPossibleValues(t).Cast<string>().ToArray();

                if (possibleValues.Any() && !possibleValues.Contains(tokenValues[t]))
                {
                    //не можем подставить токены в адрес, потому что среди них есть недопустимые
                    return url;
                }
            }

            //проверим соответствует ли адрес заданным паттернам
            var m = Match(originalUrl, targetingContext);
            if (m.IsMatch)
            {
                //адрес, очищенный от токенов
                url = m.SanitizedUrl;

                //добавим в tokenValues значения, полученные из originalUrl, по токенам, которых там не было
                foreach (var t in m.TokenValues.Keys)
                {
                    if (!tokenValues.ContainsKey(t))
                        tokenValues[t] = m.TokenValues[t];
                }
            }
            else
            {
                //не удалось наложить ни один шаблон на урл. 
                //значит, с ним ничего не поделаешь
                return url;
            }

            //для новых значений токенов определим наиболее подходящий шаблон
            var pattern = GetSuitablePattern(_config.MatchingPatterns, tokenValues);

            //добавим в урл значения токенов, согласно шаблону
            return ReplaceByPattern(url, pattern, tokenValues);
        }

        private UrlMatchingPattern GetSuitablePattern(IEnumerable<UrlMatchingPattern> patterns, Dictionary<string, string> tokenValues)
        {
            var suitable = new List<UrlMatchingPattern>();
            foreach (var pattern in patterns)
            {
                //все Defaults не должны противоречить tokenValues
                if (pattern.Defaults.Any(defaultKvp => tokenValues.ContainsKey(defaultKvp.Key) && tokenValues[defaultKvp.Key] != defaultKvp.Value))
                    break;

                //все tokenValues должны быть представлены в паттерне 
                if (tokenValues.Keys.Any(token => !pattern.Tokens.Any(t => t.Name == token) && !pattern.Defaults.Any(kvp => kvp.Key == token)))
                    break;

                suitable.Add(pattern);
            }

            if (suitable.Any())
            {
                //самым подходящим из подходящих паттернов считаем тот, у кого больше всех дефолтных значений
                //предположим есть паттерны 1) localhost/{culture} и 2) localhost с дефолтным значением culture = "ru-ru"
                //тогда если в tokenValues пришло culture = "ru-ru", нам нужно выбрать паттерн 2
                return suitable.OrderByDescending(p => p.Defaults.Count()).First();
            }

            return patterns.First();
        }

        private Url ReplaceByPattern(Url original,
            UrlMatchingPattern pattern,
            Dictionary<string, string> tokenValues)
        {
            Url url = original;
            List<string> domains = null;
            List<string> segments = original.GetSegments().ToList();

            foreach (var token in pattern.Tokens)
            {
                if (tokenValues.ContainsKey(token.Name))
                {
                    var tokenValue = tokenValues[token.Name];

                    if (token.IsInAuthority)
                    {
                        if (original.IsAbsolute)
                        {
                            if (domains == null)
                            {
                                domains = GetReversedDomains(original)
                                    .ToList();
                            }

                            if (domains.Count > token.Position)
                            {
                                domains[token.Position] = tokenValue;
                            }
                            else if (domains.Count == token.Position)
                            {
                                domains.Add(tokenValue);
                            }
                        }
                    }
                    else
                    {
                        if (segments.Count >= token.Position)
                        {
                            segments.Insert(token.Position, tokenValue);
                        }
                        else
                        {
                            segments.Add(tokenValue);
                        }
                    }
                }
            }

            if (domains != null)
            {
                url = url.SetAuthority(string.Join(".", domains.Reverse<string>()));
            }

            if (segments.Count > 0)
            {
                url = url.SetPath("/" + string.Join("/", segments));
            }

            return url;
        }

        private bool MatchToken(string[] segments, int tokenPosition, IEnumerable<string> possibleValues, out string result)
        {
            if (segments.Length > tokenPosition)
            {
                if (possibleValues.Any())
                {
                    //если токен может быть заменен на ограниченное множество возможных значений
                    //надо проверить, что значение из урла принадлежит этому множеству
                    result = MatchToken(segments[tokenPosition], possibleValues);
                    return result != null;
                }

                result = segments[tokenPosition].ToLower();
                return true;
            }
            result = null;
            return false;
        }

        private string MatchToken(string sample, IEnumerable<string> items)
        {
            if (string.IsNullOrEmpty(sample))
            {
                return null;
            }

            return items.FirstOrDefault(x => x.Equals(sample.ToLower()));
        }

        private static string[] GetReversedDomains(Url pUrl)
        {
            var domains = pUrl.Authority
                .Split('.')
                .Select(w => w.Trim())
                .Where(w => !String.IsNullOrWhiteSpace(w))
                .Reverse()
                .ToArray();
            return domains;
        }
    }
}
