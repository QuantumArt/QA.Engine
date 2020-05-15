using System;

namespace QA.DotNetCore.Engine.Abstractions.Finder
{
    public interface IItemFinder
    {
        /// <summary>
        /// Поиск элементов по условию. Применяется рекурсивный поиск по поддереву начальной страницы
        /// </summary>
        /// <param name="startPage">начальная страница для поиска</param>
        /// <param name="matchCriteria">условие поиска</param>
        /// <param name="depth">максимальная глубина поиска</param>
        /// <returns></returns>
        IAbstractItem Find(IAbstractItem startPage, Func<IAbstractItem, bool> matchCriteria, int depth = 5);

        /// <summary>
        /// Поиск элементов по условию. Применяется рекурсивный поиск по поддереву начальной страницы
        /// </summary>
        /// <param name="startPage">начальная страница для поиска</param>
        /// <param name="matchCriteria">условие поиска</param>
        /// <param name="depth">максимальная глубина поиска</param>
        /// <returns></returns>
        TAbstractItem Find<TAbstractItem>(IAbstractItem startPage, Func<TAbstractItem, bool> matchCriteria, int depth = 5) where TAbstractItem : IAbstractItem;
    }
}
