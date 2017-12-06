using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.AbTesting
{
    /// <summary>
    /// Класс, делающий выбор по AB-тестам в рамках запроса
    /// </summary>
    public class AbTestChoiceResolver
    {
        private readonly AbTestStorage _abTestStorage;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public const string QueryParamPrefix = "test-";
        public const string CookieNamePrefix = "abt-";

        private Dictionary<int, int> _currentChoices = new Dictionary<int, int>();

        public AbTestChoiceResolver(AbTestStorage abTestStorage, IHttpContextAccessor httpContextAccessor)
        {
            _abTestStorage = abTestStorage;
            _httpContextAccessor = httpContextAccessor;
        }

        public int ResolveChoice(int testId)
        {
            var request = _httpContextAccessor.HttpContext.Request;

            //возможно выбор был уже сделан с рамках обработки этого запроса
            if (_currentChoices.ContainsKey(testId))
            {
                return _currentChoices[testId];
            }

            int? choice = null;

            //возможно выбор для AB теста передан в query-параметре (приоритетнее куки)
            string queryParamValue = request.Query[QueryParamPrefix + testId];
            if (!String.IsNullOrWhiteSpace(queryParamValue))
            {
                choice = ResolveChoiceFromString(queryParamValue);
            }

            //возможно выбор для AB теста передан в cookie
            if (!choice.HasValue)
            {
                var cookie = request.Cookies[CookieNamePrefix + testId];
                if (cookie != null)
                {
                    choice = ResolveChoiceFromString(cookie);
                }
            }

            //сделаем выбор на основе рандома
            if (!choice.HasValue)
            {
                var test = _abTestStorage.GetTestById(testId);

                var percentage = test.Percentage;
                if (percentage != null && percentage.Length > 1 && percentage.Sum() > 0)
                {
                    int sum = percentage.Sum();
                    int random = new Random().Next(sum);
                    for (int i = 0; i < percentage.Length; i++)
                    {
                        random -= percentage[i];
                        if (random < 0)
                        {
                            choice = i;
                            break;
                        }
                    }
                }
            }

            //если выбор почему-то не был сделан, то делаем выбор по умолчанию
            if (!choice.HasValue)
            {
                //logger.Debug("AbTestChoiceResolver : getting default choice. testId = " + testId);
                choice = 0;
            }

            _currentChoices[testId] = choice.Value;

            return choice.Value;
        }

        private int? ResolveChoiceFromString(string str)
        {
            if (Int32.TryParse(str, out int value))
            {
                return value >= 0 ? value : default(int?);
            }
            return null;
        }
    }
}
