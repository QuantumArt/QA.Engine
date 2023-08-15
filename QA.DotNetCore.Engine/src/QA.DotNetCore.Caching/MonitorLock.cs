using System;
using System.Threading;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching;

public class MonitorLock : ILock
{
    public DateTime LastUsed { get; private set;  }

    public bool Acquire(TimeSpan timeout)
    {
        bool lockTaken = false;
        Monitor.TryEnter(this, (int) timeout.TotalMilliseconds, ref lockTaken);
        SetLastUsed(lockTaken);
        return lockTaken;
    }

    public bool Acquire()
    {
        bool lockTaken = false;
        Monitor.TryEnter(this, ref lockTaken);
        SetLastUsed(lockTaken);
        return lockTaken;
    }

    public void Release() => Monitor.Exit(this);

    private void SetLastUsed(bool lockTaken)
    {
        if (lockTaken)
        {
            LastUsed = DateTime.Now;
        }
    }
}
