using QA.DotNetCore.Engine.Routing.UrlResolve.HeadMatching;
using QA.DotNetCore.Engine.Routing.UrlResolve.TailMatching;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Routing.UrlResolve
{
    /// <summary>
    /// Конфигурация шаблонов урлов сайта.
    /// Урл любой страницы можно представить как {schema}://{host}/{path}.
    /// Урл страницы сайта, работающего на виджетной платформе можно представить в виде {schema}://{голова}/{путь от стартовой страницы}/{хвост}.
    /// "голова" - это хост сайта + опционально первые несколько сегментов пути; в этих сегментах и хосте могут быть заключены значения таргетирования (например регион, культура итд)
    /// "хвост" - это последние сегменты пути; в этих сегментах могут быть routevalues (например action, id итд)
    /// </summary>
    public class UrlTokenConfig
    {
        /// <summary>
        /// Шаблоны "головы" урла
        /// </summary>
        public List<HeadUrlMatchingPattern> HeadPatterns { get; set; }
        /// <summary>
        /// Шаблон "хвоста" урла по умолчанию для всех контроллеров
        /// </summary>
        public TailUrlMatchingPattern DefaultTailPattern { get; set; }
        /// <summary>
        /// Шаблоны "хвоста" урла, заданные индивидуально для каждого контроллера
        /// </summary>
        public Dictionary<string, List<TailUrlMatchingPattern>> TailPatternsByControllers { get; set; }
    }
}
