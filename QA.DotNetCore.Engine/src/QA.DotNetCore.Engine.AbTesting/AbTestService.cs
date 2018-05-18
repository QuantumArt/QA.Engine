using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions.OnScreen;
using QA.DotNetCore.Engine.AbTesting.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.AbTesting
{
    public class AbTestService : IAbTestService
    {
        private readonly IAbTestRepository _abTestRepository;
        private readonly ICacheProvider _cacheProvider;
        private readonly AbTestingSettings _abTestingSettings;
        private readonly IOnScreenContextProvider _onScreenContextProvider;
        private readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;

        public AbTestService(IAbTestRepository abTestRepository,
            ICacheProvider cacheProvider,
            AbTestingSettings abTestingSettings,
            IOnScreenContextProvider onScreenContextProvider,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider)
        {
            _abTestRepository = abTestRepository;
            _cacheProvider = cacheProvider;
            _abTestingSettings = abTestingSettings;
            _onScreenContextProvider = onScreenContextProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
        }

        private Dictionary<int, AbTestPersistentData> GetCachedTests()
        {
            var onScreenContext = _onScreenContextProvider.GetContext();
            var isStage = onScreenContext?.AbtestsIsStageOverrided ?? _abTestingSettings.IsStage;//isStage может быть переопределен для Onscreen
            var getOnlyActiveTests = onScreenContext == null;//в режиме Onscreen нам нужны не только активные сейчас тесты
            var cacheTags = new string[1] { _qpContentCacheTagNamingProvider.GetByNetName(_abTestRepository.AbTestNetName, _abTestingSettings.SiteId, isStage) }
                .Where(t => t != null)
                .ToArray();
            return _cacheProvider.GetOrAdd($"AbTestService.GetCachedTests_{_abTestingSettings.SiteId}_{isStage}_{getOnlyActiveTests}",
                cacheTags,
                _abTestingSettings.TestsCachePeriod,
                () => getOnlyActiveTests ?
                    _abTestRepository.GetActiveTests(_abTestingSettings.SiteId, isStage).ToDictionary(_ => _.Id) :
                    _abTestRepository.GetAllTests(_abTestingSettings.SiteId, isStage).ToDictionary(_ => _.Id));
        }

        private AbTestContainersByPaths GetCachedContainers()
        {
            var onScreenContext = _onScreenContextProvider.GetContext();
            var isStage = onScreenContext?.AbtestsIsStageOverrided ?? _abTestingSettings.IsStage;//isStage может быть переопределен для Onscreen
            var getOnlyActiveTests = onScreenContext == null;//в режиме Onscreen нам нужны не только активные сейчас тесты
            var cacheTags = new string[4] {
                _abTestRepository.AbTestNetName,
                _abTestRepository.AbTestContainerNetName,
                _abTestRepository.AbTestScriptNetName,
                _abTestRepository.AbTestRedirectNetName
            }.Select(c => _qpContentCacheTagNamingProvider.GetByNetName(c, _abTestingSettings.SiteId, isStage))
            .Where(t => t != null)
            .ToArray();

            return _cacheProvider.GetOrAdd($"AbTestService.GetCachedContainers_{_abTestingSettings.SiteId}_{isStage}_{getOnlyActiveTests}",
                cacheTags,
                _abTestingSettings.ContainersCachePeriod,
                () => new AbTestContainersByPaths(getOnlyActiveTests ?
                    _abTestRepository.GetActiveTestsContainers(_abTestingSettings.SiteId, isStage) :
                    _abTestRepository.GetAllTestsContainers(_abTestingSettings.SiteId, isStage))
                    );
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
