using QA.DotNetCore.Caching.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Caching
{
    public class CacheTagWatcher : ICacheTagWatcher
    {
        private readonly IEnumerable<ICacheTagTracker> _trackers;
        private readonly ICacheProvider _cacheProvider;
        private Dictionary<string, CacheTagModification> _modifications = new Dictionary<string, CacheTagModification>();

        public CacheTagWatcher(IEnumerable<ICacheTagTracker> trackers,
            ICacheProvider cacheProvider)
        {
            _trackers = trackers;
            _cacheProvider = cacheProvider;
        }

        public void TrackChanges()
        {
            var newValues = new Dictionary<string, CacheTagModification>();

            //собираем даты изменений кештегов по всем трекерам
            foreach (var tracker in _trackers)
            {
                foreach (var item in tracker.TrackChanges())
                {
                    newValues[item.Name] = item;
                }
            }

            if (_modifications != null && _modifications.Count > 0)
            {
                //сравниваем с предыдущим разом, поймём какие теги нужно обновлять
                var cacheTagsToUpdate = new List<string>();
                foreach (var item in newValues)
                {
                    if (!_modifications.ContainsKey(item.Key))
                    {
                        cacheTagsToUpdate.Add(item.Key);
                        continue;
                    }

                    var old = _modifications[item.Key];

                    if (old.Modified < item.Value.Modified)
                    {
                        cacheTagsToUpdate.Add(item.Key);
                    }
                }

                //инвалидируем кеш по обновившимся тегам
                if (cacheTagsToUpdate.Any())
                {
                    _cacheProvider.InvalidateByTags(cacheTagsToUpdate.ToArray());
                }

                if (newValues != null && newValues.Any())
                {
                    _modifications = newValues;
                }

                newValues = null;
            }
        }
    }
}
