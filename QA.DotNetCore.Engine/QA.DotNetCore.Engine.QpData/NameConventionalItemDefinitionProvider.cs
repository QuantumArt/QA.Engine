using Microsoft.Extensions.Options;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Persistent.Interfaces;
using QA.DotNetCore.Engine.QpData.Settings;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData
{
    public class NameConventionalItemDefinitionProvider : IItemDefinitionProvider
    {
        readonly ITypeFinder _typeFinder;
        readonly IItemDefinitionRepository _repository;
        readonly ICacheProvider _cacheProvider;
        readonly QpSettings _qpSettings;
        readonly SiteMode _siteMode;

        public NameConventionalItemDefinitionProvider(
            ITypeFinder typeFinder,
            IItemDefinitionRepository repository,
            ICacheProvider cacheProvider,
            IOptions<QpSettings> qpSettings,
            IOptions<SiteMode> siteMode)
        {
            _typeFinder = typeFinder;
            _repository = repository;
            _cacheProvider = cacheProvider;
            _qpSettings = qpSettings.Value;
            _siteMode = siteMode.Value;
        }

        public IEnumerable<ItemDefinition> GetAllDefinitions()
        {
            return GetCached().Values.ToList();
        }

        public ItemDefinition GetById(string discriminator)
        {
            var cached = GetCached();
            return cached.ContainsKey(discriminator) ? cached[discriminator] : null;
        }

        private Dictionary<string, ItemDefinition> GetCached()
        {
            return _cacheProvider.GetOrAdd("NameConventionalItemDefinitionProvider.BuildItemDefinitions", _qpSettings.CachePeriod, BuildItemDefinitions);
        }

        private Dictionary<string, ItemDefinition> BuildItemDefinitions()
        {
            var persistentData = _repository.GetAllItemDefinitions(_qpSettings.SiteId, _siteMode.IsStage);
            var typesDictionary = _typeFinder.GetTypesOf<AbstractItem>();

            return persistentData
                .Where(_ => typesDictionary.ContainsKey(_.TypeName))
                .Select(_ => new ItemDefinition { Id = _.Id, Discriminator = _.Discriminator, TypeName = _.TypeName, Type = typesDictionary[_.TypeName] })
                .ToDictionary(_ => _.Discriminator, _ => _);
        }
    }
}
