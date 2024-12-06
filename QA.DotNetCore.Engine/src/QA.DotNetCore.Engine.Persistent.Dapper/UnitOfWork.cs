using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Npgsql;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Logging;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool _disposed;
        private readonly ILogger _logger;

        public IDbConnection Connection { get; private set; }

        public DatabaseType DatabaseType { get; }

        public string CustomerCode { get; }

        public string Id { get; } = Guid.NewGuid().ToString();

        public UnitOfWork(string connectionString, string dbType, ILogger logger, string customerCode = "current")
        {
            _logger = logger;
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
            _logger.BeginScopeWith(("unitOfWorkId", Id));
            _logger.LogTrace("Creating connection for UnitOfWork");
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
                        _logger.BeginScopeWith(("unitOfWorkId", Id));
                        _logger.LogTrace("Closing connection for UnitOfWork");
                        Connection.Close();
                    }
                }

                _disposed = true;
            }
        }
    }
}
