using QA.DotNetCore.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Reflection
{
    /// <summary>
    /// реализация ITypeFinder, которая позволяет дорегистрировать типы после инициализации
    /// </summary>
    public class RegisterTypeFinder : ITypeFinder
    {
        private readonly HashSet<Type> _registeredTypes = new HashSet<Type>();

        public Dictionary<string, Type> GetTypesOf<T>()
        {
            var type = typeof(T);
            return _registeredTypes.Where(t => type.IsAssignableFrom(t)).ToDictionary(_ => _.Name, _ => _);
        }

        /// <summary>
        /// Зарегистрировать все типы из сборки, которая содержит тип T 
        /// </summary>
        /// <typeparam name="T">тип, определяющий сборку для загрузки</typeparam>
        public void RegisterFromAssemblyContaining<T>()
        {
            RegisterFromAssemblyContaining(typeof(T));
        }

        /// <summary>
        /// Зарегистрировать все типы из сборки, которая содержит тип sampleType
        /// </summary>
        /// <param name="sampleType">тип, определяющий сборку для загрузки</param>
        public void RegisterFromAssemblyContaining(Type sampleType)
        {
            foreach (var t in sampleType.Assembly.GetTypes())
            {
                _registeredTypes.Add(t);
            }
        }

        /// <summary>
        /// Зарегистрировать все типы, унаследованные от TBase, из сборки, которая содержит тип T 
        /// </summary>
        /// <typeparam name="T">тип, определяющий сборку для загрузки</typeparam>
        /// <typeparam name="TBase">базовый тип, от которого должны наследоваться загружаемые типы</typeparam>
        public void RegisterFromAssemblyContaining<T, TBase>()
        {
            RegisterFromAssemblyContaining(typeof(T), typeof(TBase));
        }

        /// <summary>
        /// Зарегистрировать все типы, унаследованные от baseType, из сборки, которая содержит тип sampleType
        /// </summary>
        /// <param name="sampleType">тип, определяющий сборку для загрузки</param>
        /// <param name="baseType">базовый тип, от которого должны наследоваться загружаемые типы</param>
        public void RegisterFromAssemblyContaining(Type sampleType, Type baseType)
        {
            foreach (var t in sampleType.Assembly.GetTypes().Where(t => baseType.IsAssignableFrom(t)))
            {
                _registeredTypes.Add(t);
            }
        }
    }
}
