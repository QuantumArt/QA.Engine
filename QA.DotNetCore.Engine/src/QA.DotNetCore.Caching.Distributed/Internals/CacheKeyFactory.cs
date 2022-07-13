namespace QA.DotNetCore.Caching.Distributed
{
    internal class CacheKeyFactory
    {
        private readonly string _instance;

        public CacheKeyFactory(string instance = null)
        {
            _instance = instance;
        }

        public CacheKey CreateKey(string key) => new(CacheKeyType.Key, key, _instance);

        public CacheKey CreateTag(string tag) => new(CacheKeyType.Tag, tag, _instance);

        public CacheKey CreateLock(CacheKey key) => new LockCacheKey(key);

        public CacheKey CreatePack(string tag) => new(CacheKeyType.Pack, tag, _instance);

        public CacheKey CreatePack(CacheKey tag) => new(CacheKeyType.Pack, tag.Key, _instance);

        public string GetPackPrefix()
        {
            string templateTagKey = "tag";
            string templatePackKey = CreatePack("tag").ToString();
            return templatePackKey.Substring(0, templatePackKey.Length - templateTagKey.Length);
        }
    }
}
