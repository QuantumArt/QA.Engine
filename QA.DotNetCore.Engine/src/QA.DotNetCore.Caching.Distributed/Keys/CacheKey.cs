using System;

namespace QA.DotNetCore.Caching.Distributed
{
    public class CacheKey : IEquatable<CacheKey>
    {
        private readonly string _fullKey;

        public CacheKeyType Type { get; }

        public string Key { get; }

        public string Instance { get; }

        public CacheKey(CacheKeyType type, string key, string instance)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));

            Type = type;
            Key = key;
            Instance = instance;
            _fullKey = $"{Type.ToString().ToLower()}:{Instance}{Key}";
        }

        public override string ToString() => _fullKey;

        public bool Equals(CacheKey other) =>
            other != null
            && Type == other.Type
            && Key.Equals(other.Key);

        public override bool Equals(object obj) => obj is CacheKey other && Equals(other);

        public override int GetHashCode() => ToString().GetHashCode();
    }
}
