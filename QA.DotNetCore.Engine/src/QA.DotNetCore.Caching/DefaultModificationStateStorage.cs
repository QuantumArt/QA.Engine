using System;
using System.Collections.Generic;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching;

public class DefaultModificationStateStorage : IModificationStateStorage
{
    private IReadOnlyCollection<CacheTagModification> _modifications = Array.Empty<CacheTagModification>();

    public void Update(TransformModificationsDelegate transformStateHandler)
    {
        _modifications = transformStateHandler(_modifications);
    }
}
