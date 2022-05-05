using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching.Distributed
{
    public class RedisNodeIdentifier : INodeIdentifier
    {
        private readonly object _syncId = new object();
        private readonly IDistributedTaggedCache _distributedTaggedCache;
        private string _id;

        public RedisNodeIdentifier(IDistributedTaggedCache distributedTaggedCache)
        {
            _distributedTaggedCache = distributedTaggedCache;
        }

        public string GetUniqueId()
        {
            if (_id != null)
                return _id;

            lock (_syncId)
            {
                string newId = _distributedTaggedCache.GetClientId();

                if (_id is null)
                    _id = newId;

                return _id;
            }
        }
    }
}
