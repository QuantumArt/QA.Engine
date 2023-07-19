using System;
using System.Threading;
using System.Threading.Tasks;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching;

public class SemaphoreAsyncLock : IAsyncLock
{
    private readonly SemaphoreSlim _semaphore;

    public SemaphoreAsyncLock(SemaphoreSlim semaphore)
    {
        _semaphore = semaphore;
    }

    public Task<bool> AcquireAsync(TimeSpan timeout) => _semaphore.WaitAsync(timeout);

    public Task<bool> AcquireAsync() => _semaphore.WaitAsync(0);

    public Task ReleaseAsync()
    {
        _semaphore.Release();
        return Task.CompletedTask;
    }
}
