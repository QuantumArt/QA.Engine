using QA.DotNetCore.Caching.Interfaces;
using System;

namespace QA.DotNetCore.Caching.Distributed;

public class DistributedModificationStateStorage : IModificationStateStorage
{
    private const string InvalidationStateKey = nameof(DistributedModificationStateStorage);
    private readonly static TimeSpan _invalidationStateExpiry = TimeSpan.FromDays(10);

    private readonly IDistributedCacheProvider _distributedCacheProvider;

    public DistributedModificationStateStorage(IDistributedCacheProvider distributedCacheProvider)
    {
        _distributedCacheProvider = distributedCacheProvider;
    }

    public void Update(TransformModificationsDelegate transformStateHandler)
    {
        var previousModifications = _distributedCacheProvider.Get<CacheTagModification[]>(InvalidationStateKey)
             ?? Array.Empty<CacheTagModification>();

        var currentModifications = transformStateHandler(previousModifications);

        _distributedCacheProvider.Set(InvalidationStateKey, currentModifications, _invalidationStateExpiry);
    }
}
