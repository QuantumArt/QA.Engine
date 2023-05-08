using System;

namespace QA.DotNetCore.Caching.Distributed;

public class ExternalCacheSettings
{
    private static readonly string _defaultInstanceName = Guid.NewGuid().ToString();
    
    public string AppName { get; set; }

    public string InstanceName { get; set; } = _defaultInstanceName;
}
