using QA.DotNetCore.Caching.Distributed.Keys;

namespace QA.DotNetCore.Caching.Distributed.Internals
{
    public class ExternalCacheKeyFactory : CacheKeyFactoryBase
    {
        private readonly ExternalCacheSettings _settings;

        public ExternalCacheKeyFactory(ExternalCacheSettings settings)
        {
            _settings = settings;
        }

        internal CacheKey CreateKey(string key) =>
            new(CacheKeyType.Key, key, _settings.AppName, _settings.InstanceName);

        internal CacheKey CreateTag(string tag) =>
            new(CacheKeyType.Tag, tag, _settings.AppName, _settings.InstanceName);

        internal CacheKey CreateLock(string key) =>
            new(CacheKeyType.Lock, key, _settings.AppName, _settings.InstanceName);

        public override string GetDataKey(string key) => !key.StartsWith(_keyPrefix) ? CreateKey(key).ToString() : key;

        public override string GetTagKey(string tag) => !tag.StartsWith(_tagPrefix) ? CreateTag(tag).ToString() : tag;

        public string GetLockKey(string key) => CreateLock(key).ToString();
    }
}
