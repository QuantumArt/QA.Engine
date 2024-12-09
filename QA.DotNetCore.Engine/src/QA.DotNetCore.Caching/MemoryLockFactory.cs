using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching;

/// <summary>
/// Object lockers and semaphores factory implementation for in-memory cache.
/// Should be registered in DI as singleton or singleton-per-tenant.
/// </summary>
public class MemoryLockFactory : ILockFactory
{
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<string, MonitorLock> _lockers = new();
    private readonly ConcurrentDictionary<string, SemaphoreAsyncLock> _semaphores = new();

    public MemoryLockFactory(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<MemoryLockFactory>();
    }

    public ILock CreateLock(string key) => _lockers.GetOrAdd(key, _ => new MonitorLock());

    public IAsyncLock CreateAsyncLock(string key) =>
        _semaphores.GetOrAdd(key, _ => new SemaphoreAsyncLock(new SemaphoreSlim(1)));

    public void DeleteLocksOlderThan(DateTime dateTime)
    {
        var lockerKeys = _lockers.Where(n => n.Value.LastUsed < dateTime)
            .Select(n => n.Key).ToList();

        _logger.LogInformation("Deleting {toDelete} lockers from {total}", lockerKeys.Count, _lockers.Count);
        lockerKeys.ForEach(key => _lockers.Remove(key, out _));

        var semaphoreKeys = _semaphores.Where(n => n.Value.LastUsed < dateTime)
            .Select(n => n.Key).ToList();
        _logger.LogInformation("Deleting {toDelete} semaphores from {total}", semaphoreKeys.Count, _semaphores.Count);
        semaphoreKeys.ForEach(key =>
        {
            _semaphores.Remove(key, out SemaphoreAsyncLock semaphoreAsyncLock);
            if (semaphoreAsyncLock is { InUse: false })
            {
                semaphoreAsyncLock.Dispose();
            }
        });

    }
}
