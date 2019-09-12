using System;
using System.Collections.Generic;

namespace  QA.DotNetCore.Engine.Persistent.Interfaces.Data
{
    /// <summary>
    /// Коллекция полей-расширений для AbstractItem (позаимствовано из QA.Core.Engine)
    /// </summary>
    public class AbstractItemExtensionCollection
    {
        readonly Dictionary<string, InnerItem> _innerDictionary;

        public AbstractItemExtensionCollection()
        {
            _innerDictionary = new Dictionary<string, InnerItem>();
        }

        public void Add(string key, object value)
        {
            if (!_innerDictionary.ContainsKey(key.ToUpper()))
                _innerDictionary.Add(key.ToUpper(), new InnerItem(value));
        }

        public void Set(string key, object value)
        {
            if (_innerDictionary.ContainsKey(key.ToUpper()))
                _innerDictionary[key.ToUpper()].Value = value;
        }

        public ICollection<string> Keys => _innerDictionary.Keys;

        public object this[string key]
        {
            get => _innerDictionary[key.ToUpper()].Value;
            set => _innerDictionary[key.ToUpper()] = new InnerItem(value);
        }

        public object Get(string key, Type type)
        {
            if (!_innerDictionary.ContainsKey(key.ToUpper()))
                return null;

            var value = _innerDictionary[key.ToUpper()].Value;
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

        public bool ContainsKey(string key)
        {
            return _innerDictionary.ContainsKey(key.ToUpper());
        }

        public int Count => _innerDictionary.Count;

        internal class InnerItem
        {
            public object Value { get; set; }
            public InnerItem(object obj)
            {
                Value = obj;
            }
        }
    }
}
