using QA.DotNetCore.Caching.Interfaces;
using RedLockNet;
using RedLockNet.SERedis;

namespace QA.DotNetCore.Caching.Distributed;

public class ExternalLockFactory : ILockFactory
{
    private readonly IDistributedLockFactory _lockFactory;
    
    public ExternalLockFactory(IDistributedLockFactory lockFactory)
    {
        _lockFactory = lockFactory;
    }

    public ILock CreateLock(string key) => new ExternalLock(key, _lockFactory);

    public IAsyncLock CreateAsyncLock(string key) => new ExternalLock(key, _lockFactory);
}
