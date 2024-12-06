using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Logging;

namespace QA.DotNetCore.Caching
{
    public class CacheTagWatcher : ICacheTagWatcher
    {
        private readonly ILogger _logger;
        private readonly ICacheTrackersAccessor _trackersAccessor;
        private readonly ICacheInvalidator _cacheInvalidator;
        private readonly IModificationStateStorage _modificationStateStorage;
        private readonly IServiceProvider _provider;

        public CacheTagWatcher(
            ICacheTrackersAccessor trackersAccessor,
            ICacheInvalidator cacheInvalidator,
            IModificationStateStorage modificationStateStorage,
            IServiceProvider provider,
            ILogger<CacheTagWatcher> logger)
        {
            _trackersAccessor = trackersAccessor;
            _cacheInvalidator = cacheInvalidator;
            _modificationStateStorage = modificationStateStorage;
            _provider = provider;
            _logger = logger;
        }

        public void TrackChanges()
        {
            var checkId = Guid.NewGuid().ToString();
            _logger.BeginScopeWith(("invalidationId", checkId));
            _logger.LogInformation("Invalidation started");
            _modificationStateStorage.Update(previousModifications =>
            {
                try
                {
                    var currentModifications = GetCurrentCacheTagModifications();
                    var cacheTagsToInvalidate = GetCacheTagsToInvalidate(previousModifications, currentModifications);
                    InvalidateTags(cacheTagsToInvalidate, checkId);
                    _logger.LogInformation("Invalidation completed");
                    return currentModifications;
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Invalidation failed");
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

            var modifications = changedModifications
                .Select(n => n.ToString())
                .ToArray();
            _logger.BeginScopeWith(("modifications", modifications));
            _logger.LogTrace("Changed modifications");

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
            var scopeData = new Dictionary<string, object> { { "invalidationId", checkId } };
            using var logScope = _logger.BeginScope(scopeData);
            if (cacheTagsToInvalidate.Length > 0)
            {
                var scopeData2 = new Dictionary<string, object> { { "tags", cacheTagsToInvalidate } };
                using var logScope2 = _logger.BeginScope(scopeData2);
                _logger.LogInformation("Invalidate tags");
                _cacheInvalidator.InvalidateByTags(cacheTagsToInvalidate.ToArray());
            }
            else
            {
                _logger.LogInformation("No tags are invalidated");
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
