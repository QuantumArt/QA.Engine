using System.Data;

namespace  QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IUnitOfWork
    {
        IDbConnection Connection { get; }
        DatabaseType DatabaseType { get; }
    }
}
