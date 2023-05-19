using QA.DotNetCore.Caching.Interfaces;
using RedLockNet.SERedis;

namespace QA.DotNetCore.Caching.Distributed;

public class DistributedLockFactory : ILockFactory
{
    private readonly RedLockFactory _redLockFactory;
    
    public DistributedLockFactory(RedLockFactory redLockFactory)
    {
        _redLockFactory = redLockFactory;
    }

    public ILock CreateLock(string key) => new DistributedLock(key, _redLockFactory);

    public IAsyncLock CreateAsyncLock(string key) => new DistributedLock(key, _redLockFactory);
}
