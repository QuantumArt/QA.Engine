using QA.DotNetCore.Caching.Interfaces;
using RedLockNet;
using RedLockNet.SERedis;

namespace QA.DotNetCore.Caching.Distributed;

public class ExternalLockFactory : ILockFactory
{
    private readonly IDistributedLockFactory _lockFactory;
    private readonly ExternalCacheSettings _settings;
    
    public ExternalLockFactory(IDistributedLockFactory lockFactory, ExternalCacheSettings settings)
    {
        _lockFactory = lockFactory;
        _settings = settings;
    }

    public ILock CreateLock(string key) => new ExternalLock(key, _lockFactory, _settings);

    public IAsyncLock CreateAsyncLock(string key) => new ExternalLock(key, _lockFactory, _settings);
}
