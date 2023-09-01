using QA.DotNetCore.Engine.Persistent.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using Npgsql;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class UnitOfWork : IUnitOfWork
    {
        private bool disposed = false;
        public IDbConnection Connection { get; private set; }
        public DatabaseType DatabaseType { get; }

        public string CustomerCode { get; }

        public UnitOfWork(string connectionString, string dbType = "MSSQL", string customerCode = "current")
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
