using System;

namespace QA.DotNetCore.Engine.QpData.Replacements
{
    /// <summary>
    /// Опция загрузки структуры сайта, заменяющая имя файла в библиотеке QP на полный путь (урл) до него
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    [Obsolete]
    public class LibraryUrlAttribute : Attribute
    {
        public LibraryUrlAttribute()
        {
        }

        public LibraryUrlAttribute(string qpPropertyName)
        {
            PropertyName = qpPropertyName;
        }
        /// <summary>
        /// Название св-ва в QP
        /// </summary>
        public string PropertyName { get; private set; }
    }
}
