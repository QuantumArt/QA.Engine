using System;

namespace QA.DotNetCore.Caching.Distributed;

public class ExternalCacheSettings
{
    private static readonly string _defaultInstanceName = Guid.NewGuid().ToString();

    public string AppName { get; set; }

    public string InstanceName { get; set; } = _defaultInstanceName;

    public bool UseExternalLock { get; set; } = true;

    public TimeSpan LockExpireTimeout { get; set; } = TimeSpan.FromMinutes(2);

    public TimeSpan LockRetryInterval { get; set; } = TimeSpan.FromSeconds(5);
}

