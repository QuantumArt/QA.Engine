using System;
using NUnit.Framework;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure
{
    internal static class EnvHelpers
    {
        private const string CiDbNameParamPrefix = "wp_test_ci_";

        private const string CiDbNameParam = CiDbNameParamPrefix + "dbname";
        private const string CiDbServerParam = CiDbNameParamPrefix + "dbserver";
        private const string CiPgLoginParam = CiDbNameParamPrefix + "pg_login";
        private const string CiPgPasswordParam = CiDbNameParamPrefix + "pg_password";
        private const string CiSqlLoginParam = CiDbNameParamPrefix + "sql_login";
        private const string CiSqlPasswordParam = CiDbNameParamPrefix + "sql_password";

        private static readonly string _ciLocalDbName = $"{CiDbNameParamPrefix}{Environment.MachineName}";

        internal static string DbNameToRunTests =>
            TestContext.Parameters.Get(CiDbNameParam, _ciLocalDbName)?.ToLowerInvariant();

        internal static string DbServerToRunTests =>
            TestContext.Parameters.Get(CiDbServerParam);

        internal static string PgDbLoginToRunTests =>  TestContext.Parameters.Get(CiPgLoginParam);

        internal static string PgDbPasswordToRunTests =>  TestContext.Parameters.Get(CiPgPasswordParam);

        internal static string SqlDbLoginToRunTests =>  TestContext.Parameters.Get(CiSqlLoginParam);

        internal static string SqlDbPasswordToRunTests =>  TestContext.Parameters.Get(CiSqlPasswordParam);
    }
}
