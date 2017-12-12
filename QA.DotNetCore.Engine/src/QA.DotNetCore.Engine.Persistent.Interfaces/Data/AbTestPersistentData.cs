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
        /// Шаблоны страниц, которыми ограничен тест
        /// </summary>
        public string UrlPatterns { get; set; }

        /// <summary>
        /// Дата начала
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Дата окончания
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}
