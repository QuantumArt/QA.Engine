using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private bool disposed = false;

        public IDbConnection Connection { get; private set; }

        public UnitOfWork(string connectionString)
        {
            Connection = new SqlConnection(connectionString);
            Connection.Open();
        }

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
                    if (Connection.State != ConnectionState.Closed)
                        Connection.Close();
                }

                disposed = true;
            }
        }
    }
}
