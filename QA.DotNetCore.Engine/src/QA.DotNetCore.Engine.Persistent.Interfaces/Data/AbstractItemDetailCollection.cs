using System;
using System.Collections.Generic;

namespace  QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    /// <summary>
    /// Коллекция полей-расширений для AbstractItem (позаимствовано из QA.Core.Engine)
    /// </summary>
    public class AbstractItemExtensionCollection
    {
        private Dictionary<string, object> InnerDictionary { get; set; }

        public AbstractItemExtensionCollection()
        {
            InnerDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }

        public void Add(string key, object value) => InnerDictionary.TryAdd(key, value);

        public void Set(string key, object value) => InnerDictionary[key] = value;

        public ICollection<string> Keys => InnerDictionary.Keys;

        public object this[string key]
        {
            get => InnerDictionary[key];
        }

        public object Get(string key, Type type)
        {
            if (!InnerDictionary.ContainsKey(key))
            {
                return null;
            }

            var value = InnerDictionary[key];
            if (type == typeof(string))
            {
                return Convert.ToString(value);
            }
            if (value == null)
            {
                return null;
            }
            if (type == typeof(double) || type == typeof(double?))
            {
                return Convert.ToDouble(value);
            }
            if (type == typeof(int) || type == typeof(int?))
            {
                return Convert.ToInt32(value);
            }
            if (type == typeof(long) || type == typeof(long?))
            {
                return Convert.ToInt64(value);
            }
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return Convert.ToDateTime(value);
            }
            if (type == typeof(bool) || type == typeof(bool?))
            {
                return Convert.ToBoolean(value);
            }

            return value;
        }

        public bool ContainsKey(string key) => InnerDictionary.ContainsKey(key);

        public int Count => InnerDictionary.Count;

    }
}
