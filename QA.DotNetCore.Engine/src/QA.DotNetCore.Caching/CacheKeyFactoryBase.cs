using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching;

public class CacheKeyFactoryBase : ICacheKeyFactory
{
    protected readonly string _keyPrefix = "key:";
    
    protected readonly string _tagPrefix = "tag:";
    
    protected readonly string _deprecatedSuffix = "__Deprecated";
    
    
    public virtual string GetDataKey(string key) => !key.StartsWith(_keyPrefix) ? _keyPrefix + key : key;

    public virtual string GetTagKey(string tag) => !tag.StartsWith(_tagPrefix)  ? _tagPrefix + tag : tag;

    public virtual string GetDeprecatedKey(string key) => !key.EndsWith(_deprecatedSuffix) ? key + _deprecatedSuffix : key;
}
