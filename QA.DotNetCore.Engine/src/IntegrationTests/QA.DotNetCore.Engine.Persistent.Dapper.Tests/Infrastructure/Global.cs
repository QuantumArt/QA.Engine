using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure
{
    internal static class Global
    {
        public const int SiteId = 52;
        public static string ConnectionString
        {
            get
            {
                var basePart = $"Database={EnvHelpers.DbNameToRunTests};Server={EnvHelpers.DbServerToRunTests};Application Name=IntegrationTest;";
                if (!String.IsNullOrEmpty(EnvHelpers.PgDbLoginToRunTests))
                {
                    return $"{basePart}User Id={EnvHelpers.PgDbLoginToRunTests};Password={EnvHelpers.PgDbPasswordToRunTests}";
                }

                if (!String.IsNullOrEmpty(EnvHelpers.SqlDbLoginToRunTests))
                {
                    return $"{basePart}User Id={EnvHelpers.SqlDbLoginToRunTests};Password={EnvHelpers.SqlDbPasswordToRunTests}";
                }

                return $"{basePart}Integrated Security=True;Connection Timeout=600";
            }
        }

        public static string DbType => !String.IsNullOrEmpty(EnvHelpers.PgDbLoginToRunTests) ? "pg" : "mssql";

        public static UnitOfWork CreateConnection => new UnitOfWork(ConnectionString, DbType);

        public static IServiceProvider CreateMockServiceProviderWithConnection()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(sp => sp.GetService(typeof(IUnitOfWork))).Returns(CreateConnection);
            return mockServiceProvider.Object;
        }
    }
}
