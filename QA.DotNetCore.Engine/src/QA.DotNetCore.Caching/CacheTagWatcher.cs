using QA.DotNetCore.Caching.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System;

namespace QA.DotNetCore.Caching
{
    public class CacheTagWatcher : ICacheTagWatcher
    {
        private readonly ICacheTrackersAccessor _trackersAccessor;
        private readonly ICacheProvider _cacheProvider;
        private readonly ILogger _logger;
        private Dictionary<string, CacheTagModification> _modifications = new Dictionary<string, CacheTagModification>();

        public CacheTagWatcher(ICacheTrackersAccessor trackersAccessor,
            ICacheProvider cacheProvider,
            ILogger<CacheTagWatcher> logger)
        {
            _trackersAccessor = trackersAccessor;
            _cacheProvider = cacheProvider;
            _logger = logger;
        }

        public void TrackChanges()
        {
            var checkId = Guid.NewGuid();
            _logger.LogTrace($"Invalidation {checkId} started");

            var trackers = _trackersAccessor.Get();
            if (trackers != null && trackers.Any())
            {
                var newValues = new Dictionary<string, CacheTagModification>();

                //собираем даты изменений кештегов по всем трекерам
                foreach (var tracker in trackers)
                {
                    foreach (var item in tracker.TrackChanges())
                    {
                        newValues[item.Name] = item;
                    }
                }

                if (_modifications != null && _modifications.Any())
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

                    _logger.LogTrace($"Invalidation {checkId} check result: ({String.Join(";", cacheTagsToUpdate)})");

                    //инвалидируем кеш по обновившимся тегам
                    if (cacheTagsToUpdate.Any())
                    {
                        _logger.LogInformation("Invalidate tags: {0}", String.Join(";", cacheTagsToUpdate));
                        _cacheProvider.InvalidateByTags(cacheTagsToUpdate.ToArray());
                    }
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
