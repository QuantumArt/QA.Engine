using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.QpData.Persistent.Data
{
    /// <summary>
    /// Коллекция полей-расширений для AbstractItem (позаимствовано из QA.Core.Engine)
    /// </summary>
    public class AbstractItemExtensionCollection
    {
        Dictionary<string, InnerItem<int, object>> _innerDictionary;

        public AbstractItemExtensionCollection()
        {
            _innerDictionary = new Dictionary<string, InnerItem<int, object>>();
        }

        public void Add(string key, object value)
        {
            _innerDictionary.Add(key, new InnerItem<int, object>(value));
        }

        public bool ContainsKey(string key)
        {
            return _innerDictionary.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _innerDictionary.Keys; }
        }

        public object this[string key]
        {
            get
            {
                return _innerDictionary[key].Value;
            }
            set
            {
                _innerDictionary[key] = new InnerItem<int, object>(value);
            }
        }

        public object Get(string key, Type type)
        {
            if (!_innerDictionary.ContainsKey(key))
                return null;

            var value = _innerDictionary[key].Value;
            if (type == typeof(string))
            {
                return Convert.ToString(value);
            }
            else if (value == null)
            {
                return null;
            }
            else if (type == typeof(double) || type == typeof(double?))
            {
                return Convert.ToDouble(value);
            }
            else if (type == typeof(int) || type == typeof(int?))
            {
                return Convert.ToInt32(value);
            }
            else if (type == typeof(long) || type == typeof(long?))
            {
                return Convert.ToInt64(value);
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return Convert.ToDateTime(value);
            }
            else if (type == typeof(bool) || type == typeof(bool?))
            {
                return Convert.ToBoolean(value);
            }

            return value;
        }

        public int Count
        {
            get { return _innerDictionary.Count; }
        }

        internal class InnerItem<TId, TValue> : IEquatable<InnerItem<TId, TValue>>
        {
            public TId Id { get; set; }
            public TValue Value { get; set; }
            public Type Type { get; set; }

            public InnerItem(TValue value) : this(value, default(TId), null) { }

            public InnerItem(TValue value, TId id) : this(value, id, null) { }

            public InnerItem(TValue item, TId id, Type type)
            {
                Value = item;
                Id = id;
                Type = type;
            }

            public bool Equals(InnerItem<TId, TValue> other)
            {
                if (other == null || Value == null || other.Value == null)
                {
                    return false;
                }

                if (object.ReferenceEquals(this, other))
                {
                    return true;
                }

                if (object.ReferenceEquals(Value, other.Value))
                {
                    return true;
                }

                return Value.Equals(other.Value);
            }

            public override int GetHashCode()
            {
                if (Value != null)
                {
                    return Value.GetHashCode();
                }
                else
                {
                    return base.GetHashCode();
                }
            }

            public override bool Equals(object obj)
            {
                if (obj is InnerItem<TId, TValue>)
                {
                    return ((IEquatable<InnerItem<TId, TValue>>)this)
                        .Equals((InnerItem<TId, TValue>)obj);
                }

                return base.Equals(obj);
            }

        }
    }
}
