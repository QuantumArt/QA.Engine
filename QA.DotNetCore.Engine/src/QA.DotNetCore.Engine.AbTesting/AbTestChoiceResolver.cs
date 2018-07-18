using Microsoft.AspNetCore.Http;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
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
        private readonly IAbTestService _abTestService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public const string QueryParamPrefix = "test-";
        public const string CookieNamePrefix = "abt-";
        public const string ForceCookieNamePrefix = "force-abt-";

        private Dictionary<int, int?> _currentChoices = new Dictionary<int, int?>();

        public AbTestChoiceResolver(IAbTestService abTestService, IHttpContextAccessor httpContextAccessor)
        {
            _abTestService = abTestService;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Делаем выбор варианта для теста в рамках запроса.
        /// </summary>
        /// <param name="test">Тест</param>
        /// <returns>null - если тест отключен (никакой вариант не выбран), иначе номер варианта.</returns>
        public int? ResolveChoice(AbTestPersistentData test)
        {
            if (test == null)
                throw new ArgumentNullException(nameof(test));

            var request = _httpContextAccessor.HttpContext.Request;

            //возможно выбор был уже сделан с рамках обработки этого запроса
            if (_currentChoices.ContainsKey(test.Id))
            {
                return _currentChoices[test.Id];
            }

            //нужно понять включен ли на самом деле тест
            //он может быть включен\выключен в QP; еще у теста есть дата начала и окончания, они тоже влияют на его включенность
            var enabled = test.Enabled;
            if (enabled)
            {
                var now = DateTime.Now;
                enabled = (!test.StartDate.HasValue || test.StartDate.Value < now) && (!test.EndDate.HasValue || now < test.EndDate.Value);
            }

            //включенность может быть переопределена для текущего запроса (с помощью специальной force-куки)
            var forceCookie = request.Cookies[ForceCookieNamePrefix + test.Id];
            if (forceCookie != null)
            {
                //в force-куки могут быть значения 0 или 1, другие игнорируем
                var force = ResolveChoiceFromString(forceCookie);
                if (force == 0 || force == 1)
                {
                    enabled = force == 1;
                }
            }

            //если тест выключен - выбор по нему не делаем
            if (!enabled)
            {
                _currentChoices[test.Id] = null;
                return null;
            }

            int? choice = null;

            //возможно выбор для AB теста передан в query-параметре (приоритетнее куки)
            string queryParamValue = request.Query[QueryParamPrefix + test.Id];
            if (!String.IsNullOrWhiteSpace(queryParamValue))
            {
                choice = ResolveChoiceFromString(queryParamValue);
            }

            //возможно выбор для AB теста передан в cookie
            if (!choice.HasValue)
            {
                var cookie = request.Cookies[CookieNamePrefix + test.Id];
                if (cookie != null)
                {
                    choice = ResolveChoiceFromString(cookie);
                }
            }

            //сделаем выбор на основе рандома
            if (!choice.HasValue)
            {
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

            _currentChoices[test.Id] = choice;

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
