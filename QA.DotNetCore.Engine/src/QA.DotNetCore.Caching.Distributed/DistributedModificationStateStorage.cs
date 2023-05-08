using QA.DotNetCore.Caching.Interfaces;
using System;

namespace QA.DotNetCore.Caching.Distributed;

public class DistributedModificationStateStorage : IModificationStateStorage
{
    private const string InvalidationStateKey = nameof(DistributedModificationStateStorage);
    private readonly static TimeSpan _invalidationStateExpiry = TimeSpan.FromDays(10);

    private readonly ICacheProvider _cacheProvider;

    public DistributedModificationStateStorage(ICacheProvider cacheProvider)
    {
        _cacheProvider = cacheProvider;
    }

    public void Update(TransformModificationsDelegate transformStateHandler)
    {
        var previousModifications = _cacheProvider.Get<CacheTagModification[]>(InvalidationStateKey)
             ?? Array.Empty<CacheTagModification>();

        var currentModifications = transformStateHandler(previousModifications);

        _cacheProvider.Set(InvalidationStateKey, currentModifications, _invalidationStateExpiry);
    }
}
