using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QA.DotNetCore.Caching.Distributed
{
    /// <summary>
    /// Configuration options for <see cref="RedisCache"/>.
    /// </summary>
    public class RedisCacheOptions : IValidatableObject
    {
        private static readonly TimeSpan s_minTagExpirationOffset = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan s_defaultTagExpirationOffset = TimeSpan.FromSeconds(5);

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
        public TimeSpan TagExpirationOffset { get; set; } = s_defaultTagExpirationOffset;

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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TagExpirationOffset < s_minTagExpirationOffset)
            {
                yield return new ValidationResult(
                    "Tag expiration offset is too low.",
                    new[] { nameof(TagExpirationOffset) });
            }
        }
    }
}
