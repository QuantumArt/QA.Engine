using System;
using System.Collections.Generic;
using System.Text;

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
            Instance = instance?.Trim();

            var keyParts = new List<string> { Type.ToString().ToLower(), Key };
            if (!string.IsNullOrEmpty(Instance))
                keyParts.Insert(0, Instance);

            _fullKey = string.Join(":", keyParts);
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
