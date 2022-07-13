using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching.Distributed
{
    public class RedisNodeIdentifier : INodeIdentifier
    {
        private readonly ILogger<RedisNodeIdentifier> _logger;
        private readonly object _syncId = new object();
        private readonly IDistributedTaggedCache _distributedTaggedCache;
        private string _id;

        public RedisNodeIdentifier(
            IDistributedTaggedCache distributedTaggedCache,
            ILogger<RedisNodeIdentifier> logger)
        {
            _distributedTaggedCache = distributedTaggedCache;
            _logger = logger;
        }

        public string GetUniqueId()
        {
            if (_id != null)
            {
                return _id;
            }

            lock (_syncId)
            {
                string newId = _distributedTaggedCache.GetClientId();

                if (_id is null)
                {
                    _logger.LogInformation("Set unique id {NodeId} for current instance.", newId);
                    _id = newId;
                }

                return _id;
            }
        }
    }
}
