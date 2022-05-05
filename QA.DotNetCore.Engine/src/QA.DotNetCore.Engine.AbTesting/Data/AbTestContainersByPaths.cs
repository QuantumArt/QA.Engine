using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.Persistent.Interfaces.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace QA.DotNetCore.Engine.AbTesting.Data
{
    [Serializable]
    internal class AbTestContainersByPaths : ISerializable
    {
        private const string EmptyDomainKey = "__empty__";
        private Dictionary<string, IReadOnlyDictionary<string, IEnumerable<AbTestContainerBasePersistentData>>> DomainsAndPaths { get; set; }

        public AbTestContainersByPaths(IEnumerable<AbTestContainerBasePersistentData> containers)
        {
            DomainsAndPaths = containers
                .GroupBy(x => string.IsNullOrWhiteSpace(x.Domain) ? EmptyDomainKey : x.Domain)
                .ToDictionary(
                    group => group.Key,
                    group => GetPatternToContainersLookup(
                        group.SelectMany(
                            container => container.AllowedUrlPatterns,
                            (container, pattern) => (container, pattern))));

            static IReadOnlyDictionary<string, IEnumerable<AbTestContainerBasePersistentData>> GetPatternToContainersLookup(
                IEnumerable<(AbTestContainerBasePersistentData container, string pattern)> containerPatternPairs)
            {
                return containerPatternPairs
                    .GroupBy(pair => pair.pattern, pair => pair.container)
                    .ToDictionary(patternGroup => patternGroup.Key, patternGroup => patternGroup.AsEnumerable());
            }
        }

        protected AbTestContainersByPaths(SerializationInfo info, StreamingContext context)
        {
            DomainsAndPaths = info.GetValue<Dictionary<string, IReadOnlyDictionary<string, IEnumerable<AbTestContainerBasePersistentData>>>>(
                nameof(DomainsAndPaths));
        }

        public IEnumerable<AbTestContainerBasePersistentData> Find(string domain, string path)
        {
            IEnumerable<AbTestContainerBasePersistentData> result = new List<AbTestContainerBasePersistentData>();

            if (DomainsAndPaths.ContainsKey(domain))
                result = result.Union(FindContainersInLookup(DomainsAndPaths[domain], path));

            if (DomainsAndPaths.ContainsKey(EmptyDomainKey))
                result = result.Union(FindContainersInLookup(DomainsAndPaths[EmptyDomainKey], path));

            return result;
        }

        private static IEnumerable<AbTestContainerBasePersistentData> FindContainersInLookup(
            IReadOnlyDictionary<string, IEnumerable<AbTestContainerBasePersistentData>> lookup, string path)
        {
            return lookup
                .Where(x => CheckUrlPattern(x.Key, path))
                .SelectMany(x => x.Value)
                .Where(x => !x.DeniedUrlPatterns
                    .Where(y => y != null)
                    .Any(pattern => CheckUrlPattern(pattern, path)));
        }

        private static bool CheckUrlPattern(string pattern, string path)
        {
            if (pattern.EndsWith("*") && path.StartsWith(pattern.TrimEnd('*').TrimEnd('/')))
                return true;
            if (pattern.TrimEnd('/') == path.TrimEnd('/'))
                return true;
            return false;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(DomainsAndPaths), DomainsAndPaths);
        }
    }
}
