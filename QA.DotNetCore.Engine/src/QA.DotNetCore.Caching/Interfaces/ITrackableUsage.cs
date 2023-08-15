using System;

namespace QA.DotNetCore.Caching.Interfaces;

public interface ITrackableUsage
{
    DateTime LastUsed { get; }
}
