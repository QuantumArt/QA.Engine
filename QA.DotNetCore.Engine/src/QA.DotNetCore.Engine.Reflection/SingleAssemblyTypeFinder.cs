using System;
using System.Linq;
using System.Collections.Generic;
using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.Reflection
{
    /// <summary>
    /// Тривиальная реализация ITypeFinder. Ищет только в одной библиотеке, определяемой объектом, который передаётся в конструкторе
    /// </summary>
    public class SingleAssemblyTypeFinder : ITypeFinder
    {
        public SingleAssemblyTypeFinder(object sample)
        {
            _sample = sample;
        }

        readonly object _sample;

        public Dictionary<string, Type> GetTypesOf<T>()
        {
            var type = typeof(T);

            return _sample.GetType().Assembly.GetTypes().Where(t => type.IsAssignableFrom(t)).ToDictionary(_ => _.Name, _ => _);
        }
    }
}
