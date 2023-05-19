using System;
using System.Diagnostics;

namespace QA.DotNetCore.Caching
{

    public class CacheInfo<TId> : IEquatable<CacheInfo<TId>>
    {
        public static readonly TimeSpan MinimalExpiry = TimeSpan.FromSeconds(0.5);
        private static readonly StringComparer _keyComparer = StringComparer.OrdinalIgnoreCase;

        public TId Id { get; }
        public string Key { get; }
        public string[] Tags { get; }
        public TimeSpan Expiration { get; }

        public CacheInfo(
            // TODO: Consider constraining with IEquatable<TId> interface to be able to ensure id uniqueness in collection.
            TId id,
            string key,
            TimeSpan expiration,
            params string[] tags)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or whitespace.", nameof(key));
            }

            FixupAndValidateExpiration(ref expiration);

            Id = id;
            Key = key;
            Tags = tags ?? Array.Empty<string>();
            Expiration = expiration;
        }

        public bool Equals(CacheInfo<TId> other) => _keyComparer.Equals(Key, other?.Key);

        public override bool Equals(object obj) => obj is CacheInfo<TId> cacheInfo && Equals(cacheInfo);

        public override int GetHashCode() => _keyComparer.GetHashCode(Key);

        public static void FixupAndValidateExpiration(ref TimeSpan expiration)
        {
            if (expiration < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(expiration),
                    expiration,
                    $"Expiration must not be less than {MinimalExpiry}.");
            }

            if (expiration < MinimalExpiry)
            {
                string invalidExpirationMessage = $"Expiration must not be less than {MinimalExpiry}.";
                Debug.Fail(invalidExpirationMessage);
                Trace.TraceWarning(invalidExpirationMessage);

                expiration = MinimalExpiry;
            }
        }
    }
}
