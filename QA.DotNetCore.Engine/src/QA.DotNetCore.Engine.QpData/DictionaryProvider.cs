using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;
using QA.DotNetCore.Engine.QpData.Interfaces;
using QA.DotNetCore.Engine.QpData.Settings;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.QpData
{
    public class DictionaryProvider : IDictionaryProvider
    {
        readonly IDictionaryItemRepository _repository;
        readonly ICacheProvider _cacheProvider;
        readonly IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;
        readonly QpSiteStructureCacheSettings _cacheSettings;
        readonly QpSiteStructureBuildSettings _buildSettings;
        readonly List<DictionarySettings> _dictionarySettings;

        public DictionaryProvider(
            IDictionaryItemRepository repository,
            ICacheProvider cacheProvider,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider,
            QpSiteStructureCacheSettings cacheSettings,
            QpSiteStructureBuildSettings buildSettings,
            List<DictionarySettings> dictionarySettings)
        {
            _repository = repository;
            _cacheProvider = cacheProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
            _cacheSettings = cacheSettings;
            _buildSettings = buildSettings;
            _dictionarySettings = dictionarySettings;
        }

        public IEnumerable<DictionaryItem> GetAllDictionaryItems(string key) => GetCached(key).Values;
        public IEnumerable<string> GetKeys() => _dictionarySettings.Select(item => item.Key);
        public IEnumerable<DictionaryItem> GetParentDictionaryItems(string key, string alias)
        {
            if (GetCached(key).TryGetValue(alias, out var item))
            {
                while (item != null)
                {
                    yield return item;
                    item = item.Parent;
                }                
            }
        }

        private Dictionary<string, DictionaryItem> GetCached(string key)
        {
            var setting = GetSetting(key);

            if (setting == null)
            {
                return new Dictionary<string, DictionaryItem>();
            }

            var cacheTags = new string[] { _qpContentCacheTagNamingProvider.GetByNetName(setting.NetName, _buildSettings.SiteId, _buildSettings.IsStage) }
                .Where(t => t != null)
                .ToArray();
            return _cacheProvider.GetOrAdd($"DictionaryProvider.BuildDictionaryItems.{key}",
                cacheTags,
                _cacheSettings.ItemDefinitionCachePeriod,
                () => BuildDictionaryItems(key)
                );
        }

        private Dictionary<string, DictionaryItem> BuildDictionaryItems(string key)
        {
            var setting = GetSetting(key);
            var persistentData = _repository
                .GetAllDictionaryItems(setting, _buildSettings.SiteId, _buildSettings.IsStage)
                .ToDictionary(item => item.Id);

            return persistentData.Values
                .Select(item => ToDictionaryItem(item, persistentData))
                .ToDictionary(item => item.Alias);
        }

        private DictionarySettings GetSetting(string key) => _dictionarySettings.FirstOrDefault(item => item.Key == key);

        private DictionaryItem GetParent(int? id, Dictionary<int, DictionaryItemPersistentData> persistentData)
        {
            if (id.HasValue && persistentData.TryGetValue(id.Value, out var parent))
            {
                return ToDictionaryItem(parent, persistentData);
            }

            return null;
        }

        private DictionaryItem ToDictionaryItem(DictionaryItemPersistentData item, Dictionary<int, DictionaryItemPersistentData> persistentData) => new DictionaryItem
        {
            Id = item.Id,
            ParentId = item.ParentId,
            Alias = item.Alias,
            Title = item.Title,
            Parent = GetParent(item.ParentId, persistentData)
        };
    }
}
