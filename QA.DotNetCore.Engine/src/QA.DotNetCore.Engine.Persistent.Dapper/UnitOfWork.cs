using QA.DotNetCore.Engine.Persistent.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using NLog;
using Npgsql;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class UnitOfWork : IUnitOfWork
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private bool _disposed;

        public IDbConnection Connection { get; private set; }

        public DatabaseType DatabaseType { get; }

        public string CustomerCode { get; }

        public string Id { get; } = Guid.NewGuid().ToString();

        public UnitOfWork(string connectionString, string dbType, string customerCode = "current")
        {
            switch (dbType.ToUpperInvariant())
            {
                case "POSTGRESQL":
                case "POSTGRES":
                case "PG":
                    Connection = new NpgsqlConnection(connectionString);
                    DatabaseType = DatabaseType.Postgres;
                    break;
                default:
                    Connection = new SqlConnection(connectionString);
                    DatabaseType = DatabaseType.SqlServer;
                    break;
            }

            CustomerCode = customerCode;
            _logger.ForTraceEvent()
                .Message("Creating connection for UnitOfWork")
                .Property("unitOfWorkId", Id)
                .Log();
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
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                    if (Connection.State != ConnectionState.Closed)
                    {
                        _logger.ForTraceEvent()
                            .Message("Closing connection for UnitOfWork")
                            .Property("unitOfWorkId", Id)
                            .Log();
                        Connection.Close();
                    }
                }

                _disposed = true;
            }
        }
    }
}
