using QA.DotNetCore.Engine.Persistent.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class UnitOfWork : IUnitOfWork
    {
        private ILogger _logger;
        private bool disposed = false;
        public IDbConnection Connection { get; private set; }
        public DatabaseType DatabaseType { get; }

        public string CustomerCode { get; }

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
            using (_logger.BeginScope(new { Environment.StackTrace }))
            {
                _logger.LogInformation($"Creating connection");
            }
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
                    {
                        using (_logger.BeginScope(new { Environment.StackTrace }))
                        {
                            _logger.LogInformation($"Closing connection.");
                        }
                        Connection.Close();
                    }
                }

                disposed = true;
            }
        }
    }
}
