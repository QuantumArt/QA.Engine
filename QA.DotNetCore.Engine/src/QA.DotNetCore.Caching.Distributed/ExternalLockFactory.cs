using System;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;
using RedLockNet;

namespace QA.DotNetCore.Caching.Distributed;

public class ExternalLockFactory : ILockFactory
{
    private readonly IDistributedLockFactory _lockFactory;
    private readonly ExternalCacheSettings _settings;
    private readonly ILogger _logger;

    public ExternalLockFactory(IDistributedLockFactory lockFactory, ExternalCacheSettings settings, ILogger logger)
    {
        _lockFactory = lockFactory;
        _settings = settings;
        _logger = logger;
    }

    public ExternalLockFactory(IDistributedLockFactory lockFactory, ExternalCacheSettings settings, ILogger<ExternalLockFactory> genericLogger)
        : this(lockFactory, settings, logger: genericLogger)
    {
    }

    public ILock CreateLock(string key) => new ExternalLock(key, _lockFactory, _settings);

    public IAsyncLock CreateAsyncLock(string key) => new ExternalLock(key, _lockFactory, _settings);

    public void DeleteLocksOlderThan(DateTime dateTime) => _logger.LogInformation("Deleting keys for external locks is not supported.");
}
