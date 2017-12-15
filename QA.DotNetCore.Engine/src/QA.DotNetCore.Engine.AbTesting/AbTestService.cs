using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Caching;
using System;
using System.Linq;
using System.Collections.Generic;
using QA.DotNetCore.Engine.AbTesting.Data;

namespace QA.DotNetCore.Engine.AbTesting
{
    public class AbTestService : IAbTestService
    {
        private readonly IAbTestRepository _abTestRepository;
        private readonly ICacheProvider _cacheProvider;
        private readonly AbTestingSettings _abTestingSettings;

        public AbTestService(IAbTestRepository abTestRepository, ICacheProvider cacheProvider, AbTestingSettings abTestingSettings)
        {
            _abTestRepository = abTestRepository;
            _cacheProvider = cacheProvider;
            _abTestingSettings = abTestingSettings;
        }

        private Dictionary<int, AbTestPersistentData> GetCachedTests()
        {
            return _cacheProvider.GetOrAdd("AbTestService.GetCachedTests", _abTestingSettings.TestsCachePeriod,
                () => _abTestRepository.GetActiveTests(_abTestingSettings.SiteId, _abTestingSettings.IsStage).ToDictionary(_ => _.Id));
        }

        private AbTestContainersByPaths GetCachedContainers()
        {
            return _cacheProvider.GetOrAdd("AbTestService.GetCachedContainers", _abTestingSettings.ContainersCachePeriod,
                () => new AbTestContainersByPaths(_abTestRepository.GetActiveTestsContainers(_abTestingSettings.SiteId, _abTestingSettings.IsStage)));
        }

        public AbTestPersistentData GetTestById(int testId)
        {
            var cached = GetCachedTests();
            if (cached.ContainsKey(testId))
                return cached[testId];
            return null;
        }

        public bool HasContainersForPage(string domain, string pagePath)
        {
            var cachedTests = GetCachedTests();
            var cachedContainers = GetCachedContainers();

            return cachedContainers.Find(domain, pagePath).Any(c => cachedTests.ContainsKey(c.TestId));
        }

        public AbTestWithContainers[] GetTestsWithContainersForPage(string domain, string pagePath)
        {
            var cachedTests = GetCachedTests();
            var cachedContainers = GetCachedContainers();

            var result = new Dictionary<int, AbTestWithContainers>();

            foreach (var container in cachedContainers.Find(domain, pagePath))
            {
                if (cachedTests.ContainsKey(container.TestId))
                {
                    if (!result.ContainsKey(container.TestId))
                        result[container.TestId] = new AbTestWithContainers { Test = cachedTests[container.TestId] };

                    var test = result[container.TestId];
                    if (container.Type == AbTestContainerType.Script)
                        test.ScriptContainers.Add(container as AbTestScriptContainerPersistentData);
                    if (container.Type == AbTestContainerType.ClientRedirect)
                        test.ClientRedirectContainer = container as AbTestClientRedirectContainerPersistentData;
                }
            }

            return result.Select(x => x.Value).ToArray();
        }
    }
}
