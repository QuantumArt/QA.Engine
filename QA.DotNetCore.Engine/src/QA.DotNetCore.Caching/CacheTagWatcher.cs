using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Caching
{
    public class CacheTagWatcher : ICacheTagWatcher
    {
        private readonly ICacheTrackersAccessor _trackersAccessor;
        private readonly ICacheInvalidator _cacheInvalidator;
        private readonly ILogger _logger;
        private Dictionary<string, CacheTagModification> _modifications = new Dictionary<string, CacheTagModification>();

        public CacheTagWatcher(
            ICacheTrackersAccessor trackersAccessor,
            ICacheInvalidator cacheInvalidator,
            ILogger<CacheTagWatcher> logger)
        {
            _trackersAccessor = trackersAccessor;
            _cacheInvalidator = cacheInvalidator;
            _logger = logger;
        }

        public void TrackChanges(IServiceProvider provider)
        {
            var checkId = Guid.NewGuid();
            _logger.LogTrace("Invalidation {InvalidationId} started", checkId);

            var trackers = _trackersAccessor.Get(provider);
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

                        var oldModified = _modifications[item.Key].Modified;

                        if (oldModified < item.Value.Modified)
                        {
                            cacheTagsToUpdate.Add(item.Key);
                        }
                    }

                    _logger.LogTrace(
                        "Invalidation {InvalidationId} check result: ({InvalidTags})",
                        checkId,
                        cacheTagsToUpdate);

                    //инвалидируем кеш по обновившимся тегам
                    if (cacheTagsToUpdate.Any())
                    {
                        _logger.LogInformation("Invalidate tags: {InvalidTags}", cacheTagsToUpdate);
                        _cacheInvalidator.InvalidateByTags(cacheTagsToUpdate.ToArray());
                    }
                }

                if (newValues != null && newValues.Any())
                {
                    _modifications = newValues;
                }
            }
        }
    }
}
