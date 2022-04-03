namespace QA.DotNetCore.Caching
{
    public enum CacheKeyType
    {
        /// <summary>
        /// Data key type (for storing data).
        /// </summary>
        Key,
        /// <summary>
        /// Tag key type (for keys invalidation).
        /// </summary>
        Tag,
        /// <summary>
        /// Lock key type (for optimistic locks).
        /// </summary>
        Lock,
        /// <summary>
        /// Compact tag key type (for storing number of attempts to compact a tag)
        /// </summary>
        Pack
    }
}
