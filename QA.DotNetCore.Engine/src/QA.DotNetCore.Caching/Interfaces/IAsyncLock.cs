using System;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Interfaces;

public interface IAsyncLock
{
    Task<bool> AcquireAsync();

    Task<bool> AcquireAsync(TimeSpan timeout);

    Task ReleaseAsync();
}
