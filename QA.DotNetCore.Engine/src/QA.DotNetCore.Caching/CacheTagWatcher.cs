using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using NLog;

namespace QA.DotNetCore.Caching
{
    public class CacheTagWatcher : ICacheTagWatcher
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly ICacheTrackersAccessor _trackersAccessor;
        private readonly ICacheInvalidator _cacheInvalidator;
        private readonly IModificationStateStorage _modificationStateStorage;
        private readonly IServiceProvider _provider;

        public CacheTagWatcher(
            ICacheTrackersAccessor trackersAccessor,
            ICacheInvalidator cacheInvalidator,
            IModificationStateStorage modificationStateStorage,
            IServiceProvider provider)
        {
            _trackersAccessor = trackersAccessor;
            _cacheInvalidator = cacheInvalidator;
            _modificationStateStorage = modificationStateStorage;
            _provider = provider;
        }

        public void TrackChanges()
        {
            var checkId = Guid.NewGuid().ToString();
            _logger.ForInfoEvent().Message("Invalidation started")
                .Property("invalidationId", checkId)
                .Log();

            _modificationStateStorage.Update(previousModifications =>
            {
                try
                {
                    var currentModifications = GetCurrentCacheTagModifications();
                    var cacheTagsToInvalidate = GetCacheTagsToInvalidate(previousModifications, currentModifications);
                    InvalidateTags(cacheTagsToInvalidate, checkId);
                    return currentModifications;
                }
                catch (Exception e)
                {
                    _logger.ForErrorEvent().Message("Invalidation failed")
                        .Exception(e)
                        .Property("invalidationId", checkId)
                        .Log();
                    return previousModifications;
                }
            });
        }

        private string[] GetCacheTagsToInvalidate(
            IReadOnlyCollection<CacheTagModification> previousModifications,
            IReadOnlyCollection<CacheTagModification> currentModifications)
        {
            var changedModifications = previousModifications.Count > 0
                ? currentModifications.Except(previousModifications).ToArray()
                : Array.Empty<CacheTagModification>();

            var changedModificationsString = String.Join(", ", changedModifications.Select(
                n => n.Name + " - " + n.Modified.ToLongTimeString()
            ));
            _logger.ForDebugEvent()
                .Message($"Changed modifications: ({changedModificationsString})")
                .Log();

            if (changedModifications.Length <= 0)
            {
                return Array.Empty<string>();
            }

            Dictionary<string, CacheTagModification> previousModificationMappings = previousModifications
                .ToDictionary(modification => modification.Name);

            var cacheTagsToInvalidate = new List<string>(changedModifications.Length);

            foreach (var changedModification in changedModifications)
            {
                if (previousModificationMappings.TryGetValue(changedModification.Name, out var previousModification)
                    && changedModification.Modified > previousModification.Modified)
                {
                    cacheTagsToInvalidate.Add(changedModification.Name);
                }
            }

            return cacheTagsToInvalidate.ToArray();
        }

        private void InvalidateTags(string[] cacheTagsToInvalidate, string checkId)
        {
            if (cacheTagsToInvalidate.Length > 0)
            {
                _logger.ForInfoEvent().Message("Invalidate tags")
                    .Property("tags", cacheTagsToInvalidate)
                    .Property("invalidationId", checkId)
                    .Log();
                _cacheInvalidator.InvalidateByTags(cacheTagsToInvalidate.ToArray());
            }
            else
            {
                _logger.ForInfoEvent().Message("No tags are invalidated")
                    .Property("invalidationId", checkId)
                    .Log();
            }
        }

        private HashSet<CacheTagModification> GetCurrentCacheTagModifications()
        {
            var modifications = _trackersAccessor.Get(_provider)
                .Select(tracker => tracker.TrackChanges(_provider))
                .SelectMany(modifications => modifications)
                .Reverse()
                .ToArray();

            var uniqueModificationNames = new HashSet<string>(modifications.Length);
            var aggregatedModificationsSet = new HashSet<CacheTagModification>(modifications.Length);

            foreach (var modification in modifications)
            {
                if (uniqueModificationNames.Add(modification.Name))
                {
                    _ = aggregatedModificationsSet.Add(modification);
                }
            }

            return aggregatedModificationsSet;
        }
    }
}
