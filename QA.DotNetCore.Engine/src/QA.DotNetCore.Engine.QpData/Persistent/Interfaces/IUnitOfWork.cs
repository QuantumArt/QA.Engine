using System.Data;

namespace QA.DotNetCore.Engine.QpData.Persistent.Interfaces
{
    public interface IUnitOfWork
    {
        IDbConnection Connection { get; }
    }
}
