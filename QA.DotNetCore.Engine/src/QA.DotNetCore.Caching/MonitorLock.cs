using System;
using System.Threading;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching;

public class MonitorLock : ILock
{
    public bool Acquire(TimeSpan timeout)
    {
        bool lockTaken = false;
        Monitor.TryEnter(this, (int)timeout.TotalMilliseconds, ref lockTaken);
        return lockTaken;
    }

    public bool Acquire()
    {
        bool lockTaken = false;
        Monitor.TryEnter(this, ref lockTaken);
        return lockTaken;
    }

    public void Release() => Monitor.Exit(this);
}

