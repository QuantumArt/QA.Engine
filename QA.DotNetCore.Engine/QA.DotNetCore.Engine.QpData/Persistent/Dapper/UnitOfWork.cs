using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private bool disposed = false;

        private readonly IDbConnection _connection;


        public UnitOfWork(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
            _abstractItemRepository = new Lazy<IAbstractItemRepository>(() => new AbstractItemRepository(_connection));
        }


        Lazy<IAbstractItemRepository> _abstractItemRepository;
        public IAbstractItemRepository AbstractItemRepository => _abstractItemRepository.Value;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                    if (_connection.State != ConnectionState.Closed)
                        _connection.Close();
                }

                disposed = true;
            }
        }
    }
}
