using QA.DotNetCore.Caching.Interfaces;
using System.Collections.Generic;

namespace QA.DotNetCore.Caching;

public class DefaultModificationStateStorage : IModificationStateStorage
{
    private IReadOnlyCollection<CacheTagModification> _modifications;

    public void Update(TransformModificationsDelegate transformStateHandler)
    {
        _modifications = transformStateHandler(_modifications);
    }
}
