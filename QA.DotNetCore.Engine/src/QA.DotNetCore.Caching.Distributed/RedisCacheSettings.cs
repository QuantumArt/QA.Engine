using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QA.DotNetCore.Caching.Distributed
{
    public class RedisCacheSettings : ExternalCacheSettings
    {
        private static readonly TimeSpan _defaultTagExpirationOffset = TimeSpan.FromSeconds(5);

        public bool UseCompression { get; set; }

        /// <summary>
        /// The configuration used to connect to Redis.
        /// </summary>
        [Required]
        public string Configuration { get; set; }

        /// <summary>
        /// Specifies how much longer will live a tag over its associated key.
        /// </summary>
        public TimeSpan TagExpirationOffset { get; set; } = _defaultTagExpirationOffset;

    }
}
