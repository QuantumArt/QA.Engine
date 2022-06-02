namespace QA.DotNetCore.Caching.Distributed
{
    public class CacheKeyFactory
    {
        private readonly string _instance;

        public CacheKeyFactory(string instance = null)
        {
            _instance = instance;
        }

        public CacheKey CreateKey(string key) => new CacheKey(CacheKeyType.Key, key, _instance);

        public CacheKey CreateTag(string tag) => new CacheKey(CacheKeyType.Tag, tag, _instance);

        public CacheKey CreateLock(CacheKey key) => new LockCacheKey(key);

        public CacheKey CreatePack(string tag) => new CacheKey(CacheKeyType.Pack, tag, _instance);

        public CacheKey CreatePack(CacheKey tag) => new CacheKey(CacheKeyType.Pack, tag.Key, _instance);
    }
}
