using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    public class AbTestPersistentData
    {
        public int Id { get; set; }

        /// <summary>
        /// Массив с вероятностями выбора (может быть более 2, т.е. тест может превращаться в ABN-тест)
        /// </summary>
        public int[] Percentage { get; set; }

        /// <summary>
        /// Срок истечения куки с запомненным выбром для теста (если пусто, то куки будет сессионной)
        /// </summary>
        public DateTime? CookieExpires { get; set; }

        /// <summary>
        /// Шаблоны страниц, которыми ограничен тест
        /// </summary>
        public string UrlPatterns { get; set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Устанавливать ли куки на региональный домен
        /// </summary>
        [Obsolete]
        public bool IsCookieRegional { get; set; }

        /// <summary>
        /// Синхронный ли тест (по синхронным тестам будет варьироваться output-cache)
        /// </summary>
        [Obsolete]
        public bool Synchronous { get; set; }

        [Obsolete]
        public string[] CookiePaths { get; set; }
    }
}
