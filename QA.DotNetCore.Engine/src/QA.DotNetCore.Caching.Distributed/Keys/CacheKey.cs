using System;
using System.Collections.Generic;
using System.Text;

namespace QA.DotNetCore.Caching.Distributed
{
    public class CacheKey : IEquatable<CacheKey>
    {
        public CacheKeyType Type { get; }

        public string Key { get; }

        public string Instance { get; }
        
        public string AppName { get; }

        public CacheKey(CacheKeyType type, string key, string appName, string instanceName)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            Type = type;
            Key = key;
            Instance = instanceName;
            AppName = appName;
        }

        public override string ToString()
        {
            var list = new List<string>
            {
                Type.ToString().ToLower()
            };

            if (!string.IsNullOrEmpty(AppName))
            {
                list.Add(AppName);
            }
            if (!string.IsNullOrEmpty(Instance))
            {
                list.Add(Instance);
            }
            list.Add(Type == CacheKeyType.Lock ? GetLockNumber().ToString() : Key);
            return String.Join(":", list);
        }

        public bool Equals(CacheKey other) =>
            other != null
            && Type == other.Type
            && Key.Equals(other.Key);

        public override bool Equals(object obj) => obj is CacheKey other && Equals(other);

        public override int GetHashCode() => ToString().GetHashCode();
        
        private uint GetLockNumber()
        {
            const int maxLocksNumber = 65536;
            return (uint)GetHashCode() % maxLocksNumber;
        }

    }
}
