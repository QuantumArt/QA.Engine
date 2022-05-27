namespace QA.DotNetCore.Caching.Interfaces
{
    /// <summary>
    /// Memory cache provider interface that supports distirbuted invalidation. Combines two cache storages: distirbuted and in-memory.
    /// </summary>
    /// <remarks>
    /// Required for caching non-serializable objects that requre invalidation.
    /// An actual value is stored inside in-memory cache. Also stores node-specific link to in-memory value in distirbuted cache for invalidation.
    /// </remarks>
    public interface IDistributedMemoryCacheProvider : ICacheProvider
    {
    }
}
