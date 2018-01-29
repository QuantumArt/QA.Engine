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
        readonly QpSettings _qpSettings;
        readonly ItemDefinitionCacheSettings _itemDefinitionCacheSettings;

        public NameConventionalItemDefinitionProvider(
            ITypeFinder typeFinder,
            IItemDefinitionRepository repository,
            ICacheProvider cacheProvider,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            QpSettings qpSettings,
            ItemDefinitionCacheSettings itemDefinitionCacheSettings)
        {
            _typeFinder = typeFinder;
            _repository = repository;
            _cacheProvider = cacheProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _qpSettings = qpSettings;
            _itemDefinitionCacheSettings = itemDefinitionCacheSettings;
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
            var cacheTags = new string[1] { _qpContentCacheTagNamingProvider.GetByNetName(_repository.ItemDefinitionNetName, _qpSettings.SiteId, _qpSettings.IsStage) };
            return _cacheProvider.GetOrAdd("NameConventionalItemDefinitionProvider.BuildItemDefinitions",
                cacheTags,
                _itemDefinitionCacheSettings.CachePeriod,
                BuildItemDefinitions);
        }

        private Dictionary<string, ItemDefinition> BuildItemDefinitions()
        {
            var persistentData = _repository.GetAllItemDefinitions(_qpSettings.SiteId, _qpSettings.IsStage);
            var typesDictionary = _typeFinder.GetTypesOf<AbstractItem>();

            return persistentData
                .Where(_ => typesDictionary.ContainsKey(_.TypeName))
                .Select(_ => new ItemDefinition { Id = _.Id, Discriminator = _.Discriminator, TypeName = _.TypeName, Type = typesDictionary[_.TypeName] })
                .ToDictionary(_ => _.Discriminator, _ => _);
        }
    }
}
