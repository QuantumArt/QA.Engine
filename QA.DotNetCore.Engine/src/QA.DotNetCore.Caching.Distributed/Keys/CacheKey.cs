using System;
using System.Collections.Generic;

namespace QA.DotNetCore.Caching.Distributed.Keys
{
    public class CacheKey
    {
        public CacheKeyType Type { get; }

        public string Key { get; }

        public string Instance { get; }
        
        public string AppName { get; }

        public CacheKey(CacheKeyType type, string key, string appName, string instanceName)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException($"'{nameof(key)}' cannot be null or empty.", nameof(key));
            }

            Type = type;
            Key = key;
            Instance = instanceName;
            AppName = appName;
        }

        public override string ToString()
        {
            var list = new List<string>
            {
                Type.ToString().ToLower()
            };

            if (!string.IsNullOrEmpty(AppName))
            {
                list.Add(AppName);
            }
            if (!string.IsNullOrEmpty(Instance))
            {
                list.Add(Instance);
            }
            list.Add(Key);
            return String.Join(":", list);
        }
    }
}
