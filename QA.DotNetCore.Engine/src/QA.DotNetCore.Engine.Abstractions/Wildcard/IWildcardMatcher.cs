using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions.Wildcard
{
    public interface IWildcardMatcher
    {
        /// <summary>
        /// Получить полный список шаблонов, под которые подходит строка
        /// </summary>
        /// <param name="text">строка для проверки</param>
        /// <returns></returns>
        IEnumerable<string> Match(string text);
        /// <summary>
        ///Возвращает наименее общее правило (самый длинный шаблон), которому удовлетворяет строка
        /// </summary>
        /// <param name="text">строка для проверки</param>
        /// <returns></returns>
        string MatchLongest(string text);
    }
}
