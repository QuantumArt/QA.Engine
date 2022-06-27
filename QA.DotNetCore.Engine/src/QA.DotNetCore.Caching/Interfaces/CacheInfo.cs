using System;

namespace QA.DotNetCore.Caching.Interfaces
{
    public class CacheInfo<TId> : IEquatable<CacheInfo<TId>>
    {
        private static readonly StringComparer s_keyComparer = StringComparer.OrdinalIgnoreCase;

        public TId Id { get; }
        public string Key { get; }
        public string[] Tags { get; }
        public TimeSpan Expiration { get; }

        public CacheInfo(
            TId id,
            string key,
            TimeSpan expiration,
            params string[] tags)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
            }

            Id = id;
            Key = key;
            Tags = tags ?? Array.Empty<string>();
            Expiration = expiration;
        }

        public bool Equals(CacheInfo<TId> other) => s_keyComparer.Equals(Key, other?.Key);

        public override bool Equals(object obj) => obj is CacheInfo<TId> cacheInfo && Equals(cacheInfo);

        public override int GetHashCode() => s_keyComparer.GetHashCode(Key);
    }
}
