using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Settings;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Провайдер доступных типов элементов структуры сайта. Предполагает совпадение имени класса .Net и поля TypeName у ItemDefinition
    /// </summary>
    public class NameConventionalItemDefinitionProvider : IItemDefinitionProvider
    {
        readonly ITypeFinder _typeFinder;
        readonly IItemDefinitionRepository _repository;
        readonly ICacheProvider _cacheProvider;
        readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        readonly QpSiteStructureCacheSettings _cacheSettings;
        readonly QpSiteStructureBuildSettings _buildSettings;

        public NameConventionalItemDefinitionProvider(
            ITypeFinder typeFinder,
            IItemDefinitionRepository repository,
            ICacheProvider cacheProvider,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            QpSiteStructureCacheSettings cacheSettings,
            QpSiteStructureBuildSettings buildSettings)
        {
            _typeFinder = typeFinder;
            _repository = repository;
            _cacheProvider = cacheProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheSettings = cacheSettings;
            _buildSettings = buildSettings;
        }

        public IEnumerable<IItemDefinition> GetAllDefinitions()
        {
            return GetCached().Values.Cast<IItemDefinition>().ToList();
        }

        public IItemDefinition GetById(string discriminator)
        {
            var cached = GetCached();
            return cached.ContainsKey(discriminator) ? cached[discriminator] : null;
        }

        private Dictionary<string, ItemDefinition> GetCached()
        {
            var cacheTags = new string[1] { _qpContentCacheTagNamingProvider.GetByNetName(KnownNetNames.ItemDefinition, _buildSettings.SiteId, _buildSettings.IsStage) }
                .Where(t => t != null)
                .ToArray();
            return _cacheProvider.GetOrAdd("NameConventionalItemDefinitionProvider.BuildItemDefinitions",
                cacheTags,
                _cacheSettings.ItemDefinitionCachePeriod,
                BuildItemDefinitions);
        }

        private Dictionary<string, ItemDefinition> BuildItemDefinitions()
        {
            var persistentData = _repository.GetAllItemDefinitions(_buildSettings.SiteId, _buildSettings.IsStage);
            var typesDictionary = _typeFinder.GetTypesOf<AbstractItem>();

            return persistentData
                .Where(_ => typesDictionary.ContainsKey(_.TypeName))
                .Select(_ => new ItemDefinition { Id = _.Id, Discriminator = _.Discriminator, TypeName = _.TypeName, Type = typesDictionary[_.TypeName] })
                .ToDictionary(_ => _.Discriminator, _ => _);
        }
    }
}
