using System;
using System.Threading;
using System.Threading.Tasks;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching;

public class SemaphoreAsyncLock : IAsyncLock, ITrackableUsage, IDisposable
{
    private readonly SemaphoreSlim _semaphore;

    public SemaphoreAsyncLock(SemaphoreSlim semaphore)
    {
        _semaphore = semaphore;
    }

    public DateTime LastUsed { get; private set; }
    public bool InUse => _semaphore.CurrentCount == 0;

    public async Task<bool> AcquireAsync(TimeSpan timeout)
    {
        var result = await _semaphore.WaitAsync(timeout);
        SetLastUsed(result);
        return result;
    }

    public async Task<bool> AcquireAsync()
    {
        var result = await _semaphore.WaitAsync(0);
        SetLastUsed(result);
        return result;
    }

    public Task ReleaseAsync()
    {
        _semaphore.Release();
        return Task.CompletedTask;
    }

    public void Dispose() => _semaphore?.Dispose();

    private void SetLastUsed(bool lockTaken)
    {
        if (lockTaken)
        {
            LastUsed = DateTime.Now;
        }
    }

}
