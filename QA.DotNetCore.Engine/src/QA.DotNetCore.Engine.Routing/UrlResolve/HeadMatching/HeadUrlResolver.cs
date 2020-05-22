using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching
{
    public interface IHeadUrlResolver
    {
        string SanitizeUrl(string originalUrl);
        string AddTokensToUrl(string originalUrl, Dictionary<string, string> tokens);
        Dictionary<string, string> ResolveTokenValues(string url);
    }

    public class HeadUrlResolver : IHeadUrlResolver
    {
        private readonly UrlTokenConfig _config;
        private readonly IHeadTokenPossibleValuesAccessor _headTokenPossibleValuesAccessor;

        public HeadUrlResolver(UrlTokenConfig config, IHeadTokenPossibleValuesAccessor headTokenPossibleValuesAccessor)
        {
            _config = config;
            _headTokenPossibleValuesAccessor = headTokenPossibleValuesAccessor;
        }

        public virtual string AddTokensToUrl(string originalUrl, Dictionary<string, string> tokenValues)
        {
            Url url = originalUrl;

            if (!_config.HeadPatterns.Any())
            {
                return url;
            }

            //проверим, что все новые значения токенов являются допустимыми
            foreach (var t in tokenValues.Keys)
            {
                //считаем, что название токена совпадает с ключом системы таргетирования
                //по этому названию узнаем список возможных значений для токена (если список пуст - считаем что значение может быть любым)
                var possibleValues = _headTokenPossibleValuesAccessor.GetPossibleValues(t);

                if (possibleValues.Any() && !possibleValues.Contains(tokenValues[t]))
                {
                    //не можем подставить токены в адрес, потому что среди них есть недопустимые
                    return url;
                }
            }

            //проверим соответствует ли адрес заданным паттернам (в нём уже могут быть заданы токены)
            var m = Match(originalUrl);
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
            else if (!m.IsMatchForAuthority)
            {
                //если адрес не соответсвует паттернам из-за домена
                //пример: паттерн //{region}.test.ru/{culture} а адрес http://stage.test.ru/en-us
                //в этом примере "stage" не является допустимым значением токена region
                //подставлять токены в такой адрес мы не хотим
                return url;
            }

            //для новых значений токенов определим наиболее подходящий шаблон
            var pattern = GetSuitablePattern(_config.HeadPatterns, tokenValues);

            //добавим в урл значения токенов, согласно шаблону
            return ReplaceByPattern(url, pattern, tokenValues);
        }

        public virtual Dictionary<string, string> ResolveTokenValues(string url)
        {
            var m = Match(url);
            return m.TokenValues;
        }

        public virtual string SanitizeUrl(string originalUrl)
        {
            var m = Match(originalUrl);
            if (m.IsMatch)
            {
                return m.SanitizedUrl;
            }
            return originalUrl;
        }

        private HeadUrlMatchResult Match(string originalUrl)
        {
            Url pUrl = originalUrl;
            var suitableResults = new List<HeadUrlMatchResult>();
            string[] domains = null;

            //сначала проверим токены в домене
            foreach (var pattern in _config.HeadPatterns)
            {
                HeadUrlMatchResult result = new HeadUrlMatchResult();

                if (domains == null && pUrl.Authority != null && pattern.Tokens.Any(t => t.IsInAuthority) /*pattern.IsRegionInAuthority || pattern.IsCultureInAuthority*/)
                {
                    domains = pUrl.Authority.Split('.')
                        .Select(w => w.Trim())
                        .Where(w => !String.IsNullOrEmpty(w))
                        .Reverse()
                        .ToArray();
                }

                result.IsMatchForAuthority = true;

                foreach (var token in pattern.Tokens
                    .Where(t => t.IsInAuthority)
                    .OrderByDescending(t => t.Position))
                {
                    if (domains == null)//если originalUrl без домена, а в шаблоне в домене есть токен - проигнорируем его
                        continue;

                    var success = MatchToken(domains, token.Position, token.Name, out string r);
                    if (!success)
                    {
                        result.IsMatchForAuthority = false;
                        break;
                    }

                    if (!string.IsNullOrEmpty(r))
                    {
                        result.TokenValues[token.Name] = r;
                    }
                }

                if (!result.IsMatchForAuthority)
                    continue;

                result.Pattern = pattern;
                suitableResults.Add(result);
            }

            var pSegments = pUrl.GetSegments();

            //теперь проверим токены вне домена, подготовим очищенный от токенов урл (при этом домен не трогаем)
            foreach (var result in suitableResults)
            {
                var pattern = result.Pattern;
                Url sanitized = pUrl;

                var tokens = pattern.Tokens
                    .Where(t => !t.IsInAuthority)
                    .OrderByDescending(t => t.Position);

                if (!tokens.Any())
                    result.IsMatch = true;

                foreach (var token in tokens)
                {
                    var success = MatchToken(pSegments, token.Position, token.Name, out string r);
                    if (success)
                    {
                        result.IsMatch = true;//нашли хотя бы 1 подходящий шаблону токен - считаем что шаблон подходящий
                    }
                    else
                    {
                        continue;
                    }
                    if (!string.IsNullOrEmpty(r))
                    {
                        sanitized = sanitized.RemoveSegment(token.Position);
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

                result.SanitizedUrl = sanitized;
            }

            //если suitableResults непуст, значит были шаблоны, подходящие по Authority (домену т.е)
            var matchForAuthority = suitableResults.Any();

            //теперь фильтруем suitableResults по тому, подходят ли они по токенам, которые вне домена
            suitableResults = suitableResults.Where(r => r.IsMatch).ToList();

            //из всех результатов в первый очередь выбираем тот, в котором мы смогли получить значения всех токенов из шаблона
            if (suitableResults.Any(r => r.AllTokenFound))
                return suitableResults.First(r => r.AllTokenFound);

            //если такого не нашли, выберем результат с наибольшим кол-вом найденных значений токенов
            if (suitableResults.Any())
                return suitableResults.OrderByDescending(r => r.TokenValues.Count).First();

            //ни один шаблон не подошёл (отдельно сообщим по какой причине: домен не подходит или нет)
            return matchForAuthority ? HeadUrlMatchResult.MatchOnlyForAuthority : HeadUrlMatchResult.NotMatch;
        }

        private bool MatchToken(string[] segments, int tokenPosition, string tokenName, out string result)
        {
            if (segments.Length > tokenPosition)
            {
                if (string.IsNullOrEmpty(segments[tokenPosition]))
                {
                    result = null;
                    return false;
                }

                //считаем, что название токена совпадает с ключом системы таргетирования
                //по этому названию узнаем список возможных значений для токена (если список пуст - считаем что значение может быть любым)
                var possibleValues = _headTokenPossibleValuesAccessor.GetPossibleValues(tokenName);

                if (possibleValues.Any())
                {
                    //если токен может быть заменен на ограниченное множество возможных значений
                    //надо проверить, что значение из урла принадлежит этому множеству
                    result = possibleValues.FirstOrDefault(v => v.Equals(segments[tokenPosition], StringComparison.InvariantCultureIgnoreCase));
                    return result != null;
                }

                result = segments[tokenPosition].ToLower();
                return true;
            }
            result = null;
            return false;
        }

        private HeadUrlMatchingPattern GetSuitablePattern(IEnumerable<HeadUrlMatchingPattern> patterns, Dictionary<string, string> tokenValues)
        {
            var suitable = new List<HeadUrlMatchingPattern>();
            foreach (var pattern in patterns)
            {
                //все Defaults не должны противоречить tokenValues
                if (pattern.Defaults.Any(defaultKvp => tokenValues.ContainsKey(defaultKvp.Key) && tokenValues[defaultKvp.Key] != defaultKvp.Value))
                    continue;

                //все tokenValues должны быть представлены в паттерне 
                if (tokenValues.Keys.Any(token => !pattern.Tokens.Any(t => t.Name == token) && !pattern.Defaults.Any(kvp => kvp.Key == token)))
                    continue;

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
            HeadUrlMatchingPattern pattern,
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
                                domains = original.GetReversedDomains().ToList();
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
    }
}
