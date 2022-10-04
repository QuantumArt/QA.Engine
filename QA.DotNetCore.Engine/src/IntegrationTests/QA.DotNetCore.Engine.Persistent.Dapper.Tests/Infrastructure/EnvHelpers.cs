using System;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure
{
    internal static class EnvHelpers
    {
        private static readonly IRunSettings _runSettings = UniversalRunSettings.Load();
        //private static readonly IRunSettings _runSettings = UniversalRunSettings.Load();

        private const string CiDbNameParamPrefix = "wp_test_ci_";

        private const string CiDbNameParam = CiDbNameParamPrefix + "dbname";
        private const string CiDbServerParam = CiDbNameParamPrefix + "dbserver";
        private const string CiPgLoginParam = CiDbNameParamPrefix + "pg_login";
        private const string CiPgPasswordParam = CiDbNameParamPrefix + "pg_password";
        private const string CiSqlLoginParam = CiDbNameParamPrefix + "sql_login";
        private const string CiSqlPasswordParam = CiDbNameParamPrefix + "sql_password";

        private static readonly string _ciLocalDbName = $"{CiDbNameParamPrefix}{Environment.MachineName}";

        internal static string DbNameToRunTests =>
            _runSettings.Get(CiDbNameParam, _ciLocalDbName).ToLowerInvariant();

        internal static string DbServerToRunTests =>
            _runSettings.Get(CiDbServerParam) ?? throw new InvalidOperationException();

        internal static string PgDbLoginToRunTests => _runSettings.Get(CiPgLoginParam);

        internal static string PgDbPasswordToRunTests => _runSettings.Get(CiPgPasswordParam);

        internal static string SqlDbLoginToRunTests => _runSettings.Get(CiSqlLoginParam);

        internal static string SqlDbPasswordToRunTests => _runSettings.Get(CiSqlPasswordParam);
    }
}
