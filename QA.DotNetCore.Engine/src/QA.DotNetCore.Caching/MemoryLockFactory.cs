using System.Collections.Concurrent;
using System.Threading;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching;

public class MemoryLockFactory : ILockFactory
{
    private readonly ConcurrentDictionary<string, MonitorLock> _lockers = new();
    private readonly ConcurrentDictionary<string, SemaphoreAsyncLock> _semaphores = new();
    
    public ILock CreateLock(string key) => _lockers.GetOrAdd(key, new MonitorLock());

    public IAsyncLock CreateAsyncLock(string key) =>
        _semaphores.GetOrAdd(key, new SemaphoreAsyncLock(new SemaphoreSlim(1)));
}
