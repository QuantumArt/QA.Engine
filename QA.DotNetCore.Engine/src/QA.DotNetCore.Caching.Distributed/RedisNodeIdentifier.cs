using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;
using StackExchange.Redis;
using System.Threading;

namespace QA.DotNetCore.Caching.Distributed
{
    public class RedisNodeIdentifier : INodeIdentifier
    {
        private readonly ILogger<RedisNodeIdentifier> _logger;
        private readonly object _syncId = new();
        private readonly IDistributedTaggedCache _distributedTaggedCache;
        private string _id;

        public RedisNodeIdentifier(
            IDistributedTaggedCache distributedTaggedCache,
            ILogger<RedisNodeIdentifier> logger)
        {
            _distributedTaggedCache = distributedTaggedCache;
            _logger = logger;
        }

        public string GetUniqueId(CancellationToken token)
        {
            if (_id != null)
            {
                return _id;
            }

            const int RetryDelayMs = 500;

            while (true)
            {
                lock (_syncId)
                {
                    try
                    {
                        string newId = _distributedTaggedCache.GetClientId(default);

                        if (_id is null)
                        {
                            _logger.LogInformation("Set unique id {NodeId} for current instance.", newId);
                            _id = newId;
                        }

                        return _id;
                    }
                    catch (RedisConnectionException ex) when (!token.IsCancellationRequested)
                    {
                        _logger.LogError(
                            ex,
                            "Redis connection error. Retrying again after {RetryDelay} ms",
                            RetryDelayMs);
                    }
                }

                Thread.Sleep(RetryDelayMs);
            }
        }
    }
}
