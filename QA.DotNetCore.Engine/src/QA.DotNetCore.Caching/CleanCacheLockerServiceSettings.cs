using System;

namespace QA.DotNetCore.Caching;

public class CleanCacheLockerServiceSettings
{
    public TimeSpan RunInterval { get; set; }

    public TimeSpan CleanInterval { get; set; }
}
