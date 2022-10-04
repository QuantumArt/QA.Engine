using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QA.DotNetCore.Caching.Distributed
{
    /// <summary>
    /// Configuration options for <see cref="RedisCache"/>.
    /// </summary>
    public class RedisCacheSettings : IValidatableObject
    {
        private static readonly TimeSpan _minTagExpirationOffset = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan _minLockExpiration = TimeSpan.FromMilliseconds(100);
        private static readonly TimeSpan _defaultTagExpirationOffset = TimeSpan.FromSeconds(5);
        private static readonly TimeSpan _defaultDeprecatedCacheTimeToLive = TimeSpan.FromSeconds(20);
        private static readonly TimeSpan _defaultLockExpiration = TimeSpan.FromSeconds(12);
        private static readonly TimeSpan _defaultRetryEnterLockInverval = TimeSpan.FromMilliseconds(100);

        /// <summary>
        /// The configuration used to connect to Redis.
        /// </summary>
        [Required]
        public string Configuration { get; set; }

        /// <summary>
        /// The Redis instance name. Allows partitioning a single backend cache for use with multiple apps/services.
        /// </summary>
        public string InstanceName { get; set; }

        /// <summary>
        /// Specifies how much longer will live a tag over its associated key.
        /// </summary>
        [Required]
        public TimeSpan TagExpirationOffset { get; set; } = _defaultTagExpirationOffset;

        /// <summary>
        /// Minimum size of keys collection in tag to execute compact operation.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int CompactTagSizeThreshold { get; set; } = 100;

        /// <summary>
        /// Number of tag changes should occure between compact attempts.
        /// </summary>
        [Range(0, int.MaxValue)]
        public int CompactTagFrequency { get; set; } = 100;

        [Required]
        public TimeSpan DeprecatedCacheTimeToLive { get; set; } = _defaultDeprecatedCacheTimeToLive;

        /// <summary>
        /// Time that lock lives if client doesn't extend it (e.g. due to critical failure).
        /// </summary>
        public TimeSpan LockExpiration { get; set; } = _defaultLockExpiration;

        /// <summary>
        /// Interval to try to acquire distributed lock.
        /// </summary>
        public TimeSpan RetryEnterLockInverval { get; set; } = _defaultRetryEnterLockInverval;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TagExpirationOffset < _minTagExpirationOffset)
            {
                yield return new ValidationResult(
                    $"Tag expiration mustn't be less than {_minTagExpirationOffset}.",
                    new[] { nameof(TagExpirationOffset) });
            }

            if (DeprecatedCacheTimeToLive < TimeSpan.Zero)
            {
                yield return new ValidationResult(
                    "Deprecated cache time to live mustn't be negative.",
                    new[] { nameof(DeprecatedCacheTimeToLive) });
            }

            if (LockExpiration < _minLockExpiration)
            {
                yield return new ValidationResult(
                    $"Lock expiration mustn't be less than {_minLockExpiration}.",
                    new[] { nameof(LockExpiration) });
            }

            if (RetryEnterLockInverval < TimeSpan.Zero)
            {
                yield return new ValidationResult(
                    "Retry lock enter interval mustn't be negative.",
                    new[] { nameof(RetryEnterLockInverval) });
            }
        }
    }
}
