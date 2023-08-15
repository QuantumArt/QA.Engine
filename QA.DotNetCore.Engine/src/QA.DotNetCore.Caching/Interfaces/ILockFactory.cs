using System;

namespace QA.DotNetCore.Caching.Interfaces;

public interface ILockFactory
{
    ILock CreateLock(string key);
    IAsyncLock CreateAsyncLock(string key);
    void DeleteLocksOlderThan(DateTime dateTime);
}
