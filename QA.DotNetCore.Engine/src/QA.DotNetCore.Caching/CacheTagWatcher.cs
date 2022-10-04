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
        private readonly ILogger<CacheTagWatcher> _logger;
        private readonly IModificationStateStorage _modificationStateStorage;

        public CacheTagWatcher(
            ICacheTrackersAccessor trackersAccessor,
            ICacheInvalidator cacheInvalidator,
            ILogger<CacheTagWatcher> logger,
            IModificationStateStorage modificationStateStorage)
        {
            _trackersAccessor = trackersAccessor;
            _cacheInvalidator = cacheInvalidator;
            _logger = logger;
            _modificationStateStorage = modificationStateStorage;
        }

        public void TrackChanges(IServiceProvider provider)
        {
            var checkId = Guid.NewGuid();
            using var invalidationScope = _logger.BeginScope("InvalidationId", checkId);

            _logger.LogTrace("Invalidation started");

            _modificationStateStorage.Update((previousModifications) =>
            {
                var currentModifications = GetCurrentCacheTagModifications(provider);
                var cacheTagsToInvalidate = GetCacheTagsToInvalidate(previousModifications, currentModifications);
                InvalidateTags(cacheTagsToInvalidate);

                return currentModifications;
            });
        }

        private string[] GetCacheTagsToInvalidate(
            IReadOnlyCollection<CacheTagModification> previousModifications,
            IReadOnlyCollection<CacheTagModification> currentModifications)
        {
            var changedModifications = previousModifications.Count > 0
                ? currentModifications.Except(previousModifications).ToArray()
                : currentModifications;

            _logger.LogTrace("Changed modifications: ({InvalidTags})", changedModifications);

            if (changedModifications.Count <= 0)
            {
                return Array.Empty<string>();
            }

            Dictionary<string, CacheTagModification> previousModificationMappings = previousModifications
                .ToDictionary(modification => modification.Name);

            var cacheTagsToInvalidate = new List<string>(changedModifications.Count);

            foreach (var changedModification in changedModifications)
            {
                if (!previousModificationMappings.TryGetValue(changedModification.Name, out var previousModification)
                    || changedModification.Modified > previousModification.Modified)
                {
                    cacheTagsToInvalidate.Add(changedModification.Name);
                }
            }

            return cacheTagsToInvalidate.ToArray();
        }

        private void InvalidateTags(string[] cacheTagsToInvalidate)
        {
            if (cacheTagsToInvalidate.Length > 0)
            {
                _logger.LogInformation("Invalidate tags: {InvalidTags}", cacheTagsToInvalidate);
                _cacheInvalidator.InvalidateByTags(cacheTagsToInvalidate.ToArray());
            }
            else
            {
                _logger.LogInformation("No tags are invalidated");
            }
        }

        private HashSet<CacheTagModification> GetCurrentCacheTagModifications(IServiceProvider provider)
        {
            var modifications = _trackersAccessor.Get(provider)
                .Select(tracker => tracker.TrackChanges())
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
