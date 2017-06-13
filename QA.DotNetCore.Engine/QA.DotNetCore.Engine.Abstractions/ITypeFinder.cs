using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions
{
    /// <summary>
    /// Интерфейс, позволяющий получить типы, реализующие какой-то базовый класс или интерфейс
    /// </summary>
    public interface ITypeFinder
    {
        Dictionary<string, Type> GetTypesOf<T>();
    }
}
