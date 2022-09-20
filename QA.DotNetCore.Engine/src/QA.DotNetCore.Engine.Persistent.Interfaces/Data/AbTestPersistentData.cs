using System;
using System.Linq;

namespace QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    public class AbTestPersistentData
    {
        public int Id { get; set; }

        /// <summary>
        /// Массив с вероятностями выбора (может быть более 2, т.е. тест может превращаться в ABN-тест)
        /// </summary>
        public int[] Percentage
        {
            get
            {
                var percentage = Array.Empty<int>();
                if (PercentageStr != null)
                {
                    percentage = PercentageStr.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(_ => Int32.TryParse(_.Trim(), out int tmp) ? tmp : 0)
                        .Select(_ => _ < 0 ? 0 : _)
                        .ToArray();
                }
                return percentage;
            }
            set
            {
                PercentageStr = String.Join(",", value);
            }
        }

        /// <summary>
        /// Включен ли тест
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Строковое представление поля <see cref="Percentage"/>
        /// </summary>
        public string PercentageStr { get; set; }

        /// <summary>
        /// Название теста
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Комментарий к тесту
        /// </summary>
        public string Comment { get; set; }

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
