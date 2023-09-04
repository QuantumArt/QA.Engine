using System;
using System.Data;

namespace  QA.DotNetCore.Engine.Persistent.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDbConnection Connection { get; }
        DatabaseType DatabaseType { get; }

        string CustomerCode { get; }
    }
}
