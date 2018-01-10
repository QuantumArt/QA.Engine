using System;

namespace QA.DotNetCore.Engine.Abstractions.OnScreen
{
    [Flags]
    public enum OnScreenFeatures
    {
        /// <summary>
        /// OnScreen недоступен для сайта
        /// </summary>
        None = 0,
        /// <summary>
        /// Доступна фича OnScreen для работы с виджетами и зонами
        /// </summary>
        Widgets = 1,
        /// <summary>
        /// Доступна фича OnScreen для работы с AB-тестами
        /// </summary>
        AbTests = 2
    }
}
