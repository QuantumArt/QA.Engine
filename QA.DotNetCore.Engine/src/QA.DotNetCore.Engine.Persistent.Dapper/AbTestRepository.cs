using Dapper;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace QA.DotNetCore.Engine.Persistent.Dapper
{
    public class AbTestRepository : IAbTestRepository
    {
        private readonly IDbConnection _connection;
        private readonly INetNameQueryAnalyzer _netNameQueryAnalyzer;

        public AbTestRepository(IUnitOfWork uow, INetNameQueryAnalyzer netNameQueryAnalyzer)
        {
            _connection = uow.Connection;
            _netNameQueryAnalyzer = netNameQueryAnalyzer;
        }

        //запросы с использованием NetName таблиц и столбцов
        private const string CmdGetActiveTests = @"
SELECT
    t.content_item_id AS Id,
    t.[|AbTest.Title|] as Title,
    t.[|AbTest.Percentage|] as PercentageStr,
    t.[|AbTest.StartDate|] as StartDate,
    t.[|AbTest.EndDate|] as EndDate,
    t.[|AbTest.Comment|] as Comment,
    t.[|AbTest.Enabled|] as Enabled
FROM [|AbTest|] t
WHERE (t.[|AbTest.StartDate|] IS NULL OR t.[|AbTest.StartDate|] < getdate()) AND (t.[|AbTest.EndDate|] IS NULL OR getdate() < t.[|AbTest.EndDate|])
";

        private const string CmdGetActiveTestsContainers = @"
SELECT
    cont.content_item_id AS Id,
    cont.[|AbTestBaseContainer.ParentTest|] as TestId,
    cont.[|AbTestBaseContainer.Description|] as Description,
    cont.[|AbTestBaseContainer.AllowedUrlPatterns|] as AllowedUrlPatternsStr,
    cont.[|AbTestBaseContainer.DeniedUrlPatterns|] as DeniedUrlPatternsStr,
    cont.[|AbTestBaseContainer.Domain|] as Domain,
    cont.[|AbTestBaseContainer.Precondition|] as Precondition,
    cont.[|AbTestBaseContainer.Arguments|] as Arguments
FROM [|AbTestBaseContainer|] cont
JOIN [|AbTest|] t on t.content_item_id = cont.[|AbTestBaseContainer.ParentTest|]
WHERE cont.[|AbTestBaseContainer.Type|] = (SELECT TOP 1 CONTENT_ID FROM CONTENT WHERE NET_CONTENT_NAME = N'{0}')
    AND (t.[|AbTest.StartDate|] IS NULL OR t.[|AbTest.StartDate|] < getdate()) AND (t.[|AbTest.EndDate|] IS NULL OR getdate() < t.[|AbTest.EndDate|])
";

        private const string CmdGetAbTestScripts = @"
SELECT
    s.content_item_id as Id,
    scont.[|AbTestScriptContainer.BaseContainer|] as ContainerId,
    s.[|AbTestScript.VersionNumber|] as VersionNumber,
    s.[|AbTestScript.ScriptText|] as ScriptText,
    s.[|AbTestScript.Description|] as Description
FROM [|AbTestScript|] s
JOIN [|AbTestScriptContainer|] scont on scont.content_item_id = s.[|AbTestScript.Container|]
";
        private const string CmdGetAbTestClientRedirects = @"
SELECT
    r.content_item_id as Id,
    rcont.[|AbTestClientRedirectContainer.BaseContainer|] as ContainerId,
    r.[|AbTestClientRedirect.VersionNumber|] as VersionNumber,
    r.[|AbTestClientRedirect.RedirectUrl|] as RedirectUrl
FROM [|AbTestClientRedirect|] r
JOIN [|AbTestClientRedirectContainer|] rcont on rcont.content_item_id = r.[|AbTestClientRedirect.Container|]
";

        public IEnumerable<AbTestPersistentData> GetActiveTests(int siteId, bool isStage)
        {
            var query = _netNameQueryAnalyzer.PrepareQuery(CmdGetActiveTests, siteId, isStage);
            return _connection.Query<AbTestPersistentData>(query);
        }

        public IEnumerable<AbTestContainerBasePersistentData> GetActiveTestsContainers(int siteId, bool isStage)
        {
            var scriptContainersQuery = _netNameQueryAnalyzer.PrepareQuery(String.Format(CmdGetActiveTestsContainers, "AbTestScriptContainer"), siteId, isStage);
            var scriptContainersDict = _connection.Query<AbTestScriptContainerPersistentData>(scriptContainersQuery).ToDictionary(_ => _.Id);

            var scriptQuery = _netNameQueryAnalyzer.PrepareQuery(CmdGetAbTestScripts, siteId, isStage);
            var scripts = _connection.Query<AbTestScriptPersistentData>(scriptQuery);

            foreach (var s in scripts)
            {
                if (scriptContainersDict.ContainsKey(s.ContainerId))
                {
                    scriptContainersDict[s.ContainerId].Scripts.Add(s);
                }
            }

            var redirectContainersQuery = _netNameQueryAnalyzer.PrepareQuery(String.Format(CmdGetActiveTestsContainers, "AbTestClientRedirectContainer"), siteId, isStage);
            var redirectContainersDict = _connection.Query<AbTestClientRedirectContainerPersistentData>(redirectContainersQuery).ToDictionary(_ => _.Id);

            var redirectQuery = _netNameQueryAnalyzer.PrepareQuery(CmdGetAbTestClientRedirects, siteId, isStage);
            var redirects = _connection.Query<AbTestClientRedirectPersistentData>(redirectQuery);

            foreach (var r in redirects)
            {
                if (redirectContainersDict.ContainsKey(r.ContainerId))
                {
                    redirectContainersDict[r.ContainerId].Redirects.Add(r);
                }
            }

            return scriptContainersDict.Values.Cast<AbTestContainerBasePersistentData>()
                .Union(redirectContainersDict.Values.Cast<AbTestContainerBasePersistentData>());
        }
    }
}
