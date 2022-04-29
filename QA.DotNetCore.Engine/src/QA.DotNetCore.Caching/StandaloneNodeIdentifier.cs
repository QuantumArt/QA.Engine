using QA.DotNetCore.Caching.Interfaces;
using System;

namespace QA.DotNetCore.Caching
{
    public class StandaloneNodeIdentifier : INodeIdentifier
    {
        private static readonly object _syncRoot = new object();
        private static StandaloneNodeIdentifier _instance;

        private readonly string _guid;

        public static StandaloneNodeIdentifier Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                var guid = Guid.NewGuid().ToString();

                lock (_syncRoot)
                {
                    if (_instance is null)
                        _instance = new StandaloneNodeIdentifier(guid);

                    return _instance;
                }
            }
        }

        private StandaloneNodeIdentifier(string guid)
        {
            _guid = guid ?? throw new ArgumentNullException(nameof(guid));
        }

        public string GetUniqueId() => _guid;
    }
}
