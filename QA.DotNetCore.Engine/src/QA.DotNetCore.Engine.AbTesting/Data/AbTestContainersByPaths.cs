using System;
using System.Collections.Generic;
using System.Linq;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;

namespace QA.DotNetCore.Engine.AbTesting.Data
{
    internal class AbTestContainersByPaths
    {
        private const string EmptyDomainKey = "__empty__";
        private Dictionary<string, ILookup<string, AbTestContainerBasePersistentData>> DomainsAndPaths { get; set; }

        public AbTestContainersByPaths(IEnumerable<AbTestContainerBasePersistentData> containers)
        {
            DomainsAndPaths = containers
                .GroupBy(x => String.IsNullOrWhiteSpace(x.Domain) ? EmptyDomainKey : x.Domain)
                .ToDictionary(
                    gr => gr.Key,
                    gr => gr.SelectMany(x => x.AllowedUrlPatterns, (c, p) => new { c, p }).ToLookup(x => x.p, el => el.c));
        }

        public IEnumerable<AbTestContainerBasePersistentData> Find(string domain, string path)
        {
            IEnumerable<AbTestContainerBasePersistentData> result = new List<AbTestContainerBasePersistentData>();

            if (DomainsAndPaths.ContainsKey(domain))
            {
                result = result.Union(FindContainersInLookup(DomainsAndPaths[domain], path));
            }
            if (DomainsAndPaths.ContainsKey(EmptyDomainKey))
            {
                result = result.Union(FindContainersInLookup(DomainsAndPaths[EmptyDomainKey], path));
            }

            return result;
        }

        private static IEnumerable<AbTestContainerBasePersistentData> FindContainersInLookup(ILookup<string, AbTestContainerBasePersistentData> lookup, string path)
        {
            return lookup.Where(x => CheckUrlPattern(x.Key, path)).SelectMany(x => x).Where(x => !x.DeniedUrlPatterns.Where(y => y != null).Any(pattern => CheckUrlPattern(pattern, path)));
        }

        private static bool CheckUrlPattern(string pattern, string path)
        {
            if (pattern.EndsWith("*") && path.StartsWith(pattern.TrimEnd('*').TrimEnd('/')))
                return true;
            if (pattern.TrimEnd('/') == path.TrimEnd('/'))
                return true;
            return false;
        }
    }
}
