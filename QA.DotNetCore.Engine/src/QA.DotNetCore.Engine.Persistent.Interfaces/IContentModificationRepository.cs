using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IContentModificationRepository
    {
        IEnumerable<QpContentModificationPersistentData> GetAll();
    }
}
