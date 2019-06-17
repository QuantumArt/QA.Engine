using QA.DotNetCore.Engine.Persistent.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;
using Npgsql;

namespace QA.DotNetCore.Engine.QpData.Persistent.Dapper
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private bool disposed = false;
        public IDbConnection Connection { get; private set; }
        public DatabaseType DatabaseType { get; }

        public UnitOfWork(string connectionString, string dbType = "mssql")
        {
            switch (dbType)
            {
                case "postgresql":
                case "postgres":
                case "pg":
                    Connection = new NpgsqlConnection(connectionString);
                    DatabaseType = DatabaseType.Postgres;
                    break;
                default:
                    Connection = new SqlConnection(connectionString);
                    DatabaseType = DatabaseType.SqlServer;
                    break;
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
                        Connection.Close();
                }

                disposed = true;
            }
        }
    }
}
