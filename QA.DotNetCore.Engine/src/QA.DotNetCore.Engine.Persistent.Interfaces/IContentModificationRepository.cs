using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;
using System.Data;

namespace QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IContentModificationRepository
    {
        IEnumerable<QpContentModificationPersistentData> GetAll(IDbTransaction transaction = null);

        void SetUnitOfWork(IUnitOfWork unitOfWork);
    }
}
