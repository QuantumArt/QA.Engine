using Dapper;
using Microsoft.Extensions.DependencyInjection;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NLog;

namespace QA.DotNetCore.Engine.Persistent.Dapper
{
    public class AbTestRepository : IAbTestRepository
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly IServiceProvider _serviceProvider;
        private readonly INetNameQueryAnalyzer _netNameQueryAnalyzer;

        public AbTestRepository(IServiceProvider serviceProvider, INetNameQueryAnalyzer netNameQueryAnalyzer)
        {
            _serviceProvider = serviceProvider;
            _netNameQueryAnalyzer = netNameQueryAnalyzer;
        }

        protected IUnitOfWork UnitOfWork
        {
            get
            {
                var uow = _serviceProvider.GetRequiredService<IUnitOfWork>();
                _logger.ForTraceEvent()
                    .Message($"Received UnitOfWork {uow.Id} from ServiceProvider")
                    .Log();
                return uow;
            }
        }

        //запросы с использованием NetName таблиц и столбцов
        private const string CmdGetTests = @"
SELECT
    t.content_item_id AS Id,
    t.|AbTest.Title| as Title,
    t.|AbTest.Percentage| as PercentageStr,
    t.|AbTest.StartDate| as StartDate,
    t.|AbTest.EndDate| as EndDate,
    t.|AbTest.Comment| as Comment,
    t.|AbTest.Enabled| as Enabled
FROM |AbTest| t
WHERE @onlyActive = 0 OR (
    @onlyActive = 1 AND
    (t.|AbTest.StartDate| IS NULL OR t.|AbTest.StartDate| < @currentDate) AND
    (t.|AbTest.EndDate| IS NULL OR @currentDate < t.|AbTest.EndDate|))
";

        private const string CmdGetTestsContainers = @"
SELECT
    cont.content_item_id AS Id,
    cont.|AbTestBaseContainer.ParentTest| as TestId,
    cont.|AbTestBaseContainer.Description| as Description,
    cont.|AbTestBaseContainer.AllowedUrlPatterns| as AllowedUrlPatternsStr,
    cont.|AbTestBaseContainer.DeniedUrlPatterns| as DeniedUrlPatternsStr,
    cont.|AbTestBaseContainer.Domain| as Domain,
    cont.|AbTestBaseContainer.Precondition| as Precondition,
    cont.|AbTestBaseContainer.Arguments| as Arguments
FROM |AbTestBaseContainer| cont
JOIN |AbTest| t on t.content_item_id = cont.|AbTestBaseContainer.ParentTest|
WHERE cont.|AbTestBaseContainer.Type| IN (SELECT CONTENT_ID FROM CONTENT WHERE NET_CONTENT_NAME = @containerType)
    AND (
        @onlyActive = 0 OR (
            @onlyActive = 1 AND
            (t.|AbTest.StartDate| IS NULL OR t.|AbTest.StartDate| < @currentDate) AND
            (t.|AbTest.EndDate| IS NULL OR @currentDate < t.|AbTest.EndDate|)
        )
    )
";

        private const string CmdGetAbTestScripts = @"
SELECT
    s.content_item_id as Id,
    scont.|AbTestScriptContainer.BaseContainer| as ContainerId,
    s.|AbTestScript.VersionNumber| as VersionNumber,
    s.|AbTestScript.ScriptText| as ScriptText,
    s.|AbTestScript.Description| as Description
FROM |AbTestScript| s
JOIN |AbTestScriptContainer| scont on scont.content_item_id = s.|AbTestScript.Container|
";
        private const string CmdGetAbTestClientRedirects = @"
SELECT
    r.content_item_id as Id,
    rcont.|AbTestClientRedirectContainer.BaseContainer| as ContainerId,
    r.|AbTestClientRedirect.VersionNumber| as VersionNumber,
    r.|AbTestClientRedirect.RedirectUrl| as RedirectUrl
FROM |AbTestClientRedirect| r
JOIN |AbTestClientRedirectContainer| rcont on rcont.content_item_id = r.|AbTestClientRedirect.Container|
";

        public string AbTestNetName => "AbTest";

        public string AbTestContainerNetName => "AbTestScriptContainer";

        public string AbTestScriptNetName => "AbTestScript";

        public string AbTestRedirectNetName => "AbTestClientRedirect";

        public IEnumerable<AbTestPersistentData> GetActiveTests(int siteId, bool isStage, IDbTransaction transaction = null)
        {
            return GetTests(siteId, isStage, true, transaction);
        }

        public IEnumerable<AbTestPersistentData> GetAllTests(int siteId, bool isStage, IDbTransaction transaction = null)
        {
            return GetTests(siteId, isStage, false, transaction);
        }

        public IEnumerable<AbTestContainerBasePersistentData> GetActiveTestsContainers(int siteId, bool isStage, IDbTransaction transaction = null)
        {
            return GetTestsContainers(siteId, isStage, true, transaction);
        }

        public IEnumerable<AbTestContainerBasePersistentData> GetAllTestsContainers(int siteId, bool isStage, IDbTransaction transaction = null)
        {
            return GetTestsContainers(siteId, isStage, false, transaction);
        }

        private IEnumerable<AbTestContainerBasePersistentData> GetTestsContainers(int siteId, bool isStage, bool onlyActive, IDbTransaction transaction = null)
        {
            var currentDate = DateTime.Now;
            var scriptContainersQuery = _netNameQueryAnalyzer.PrepareQuery(CmdGetTestsContainers, siteId, isStage);
            var scriptContainersDict = UnitOfWork.Connection.Query<AbTestScriptContainerPersistentData>(scriptContainersQuery, new { currentDate, onlyActive = (onlyActive ? 1 : 0), containerType = "AbTestScriptContainer" }, transaction).ToDictionary(_ => _.Id);

            var scriptQuery = _netNameQueryAnalyzer.PrepareQuery(CmdGetAbTestScripts, siteId, isStage);
            var scripts = UnitOfWork.Connection.Query<AbTestScriptPersistentData>(scriptQuery, transaction: transaction);

            foreach (var s in scripts)
            {
                if (scriptContainersDict.ContainsKey(s.ContainerId))
                {
                    scriptContainersDict[s.ContainerId].Scripts.Add(s);
                }
            }

            var redirectContainersQuery = _netNameQueryAnalyzer.PrepareQuery(CmdGetTestsContainers, siteId, isStage);
            var redirectContainersDict = UnitOfWork.Connection.Query<AbTestClientRedirectContainerPersistentData>(redirectContainersQuery, new { currentDate, onlyActive = (onlyActive ? 1 : 0), containerType = "AbTestClientRedirectContainer" }, transaction).ToDictionary(_ => _.Id);

            var redirectQuery = _netNameQueryAnalyzer.PrepareQuery(CmdGetAbTestClientRedirects, siteId, isStage);
            var redirects = UnitOfWork.Connection.Query<AbTestClientRedirectPersistentData>(redirectQuery, transaction: transaction);

            foreach (var r in redirects)
            {
                if (redirectContainersDict.ContainsKey(r.ContainerId))
                {
                    redirectContainersDict[r.ContainerId].Redirects.Add(r);
                }
            }

            return scriptContainersDict.Values.Cast<AbTestContainerBasePersistentData>()
                .Concat(redirectContainersDict.Values.Cast<AbTestContainerBasePersistentData>());
        }

        private IEnumerable<AbTestPersistentData> GetTests(int siteId, bool isStage, bool onlyActive, IDbTransaction transaction)
        {
            var query = _netNameQueryAnalyzer.PrepareQuery(CmdGetTests, siteId, isStage);
            return UnitOfWork.Connection.Query<AbTestPersistentData>(query, new { currentDate = DateTime.Now, onlyActive = (onlyActive ? 1 : 0) }, transaction);
        }
    }
}
