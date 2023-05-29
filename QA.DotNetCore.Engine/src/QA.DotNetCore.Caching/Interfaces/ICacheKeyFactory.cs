namespace QA.DotNetCore.Caching.Interfaces;

public interface ICacheKeyFactory
{
    string GetDataKey(string key);

    string GetTagKey(string tag);

    string GetDeprecatedKey(string key);
}
