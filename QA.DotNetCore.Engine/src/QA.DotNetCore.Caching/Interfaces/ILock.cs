using System;

namespace QA.DotNetCore.Caching.Interfaces;

public interface ILock
{
    bool Acquire();
    
    bool Acquire(TimeSpan timeout);
    
    void Release();
}
