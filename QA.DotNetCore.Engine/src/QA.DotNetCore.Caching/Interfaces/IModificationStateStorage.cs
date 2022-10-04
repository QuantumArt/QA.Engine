using System.Collections.Generic;

namespace QA.DotNetCore.Caching.Interfaces;

public delegate IReadOnlyCollection<CacheTagModification> TransformModificationsDelegate(
    IReadOnlyCollection<CacheTagModification> previousModifications);

public interface IModificationStateStorage
{
    void Update(TransformModificationsDelegate transformStateHandler);
}
