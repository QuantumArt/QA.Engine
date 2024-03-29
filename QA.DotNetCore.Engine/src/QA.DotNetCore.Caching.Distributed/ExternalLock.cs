using System;
using System.Threading.Tasks;
using QA.DotNetCore.Caching.Interfaces;
using RedLockNet;
using RedLockNet.SERedis;

namespace QA.DotNetCore.Caching.Distributed;

public class ExternalLock : ILock, IAsyncLock
{
    private readonly IDistributedLockFactory _factory;
    private readonly string _key;
    private readonly TimeSpan _expire;
    private readonly TimeSpan _retry;
    private IRedLock _lock;

    public ExternalLock(string key, IDistributedLockFactory factory, ExternalCacheSettings settings)
    {
        _key = key;
        _factory = factory;
        _expire = settings.LockExpireTimeout;
        _retry = settings.LockRetryInterval;
    }

    public bool Acquire()
    {
        _lock = _factory.CreateLock(_key, _expire);
        return _lock.IsAcquired;
    }

    public bool Acquire(TimeSpan timeout)
    {
        _lock = _factory.CreateLock(_key, _expire, timeout, _retry);
        return _lock.IsAcquired;
    }

    public async Task<bool> AcquireAsync()
    {
        _lock = await _factory.CreateLockAsync(_key, _expire);
        return _lock.IsAcquired;
    }

    public async Task<bool> AcquireAsync(TimeSpan timeout)
    {
        _lock = await _factory.CreateLockAsync(_key, _expire, timeout, _retry);
        return _lock.IsAcquired;
    }

    public async Task ReleaseAsync() => await _lock.DisposeAsync();

    public void Release() => _lock.Dispose();

}
