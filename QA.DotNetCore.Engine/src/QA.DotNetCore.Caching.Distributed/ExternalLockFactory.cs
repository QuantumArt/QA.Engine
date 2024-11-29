using System;
using NLog;
using QA.DotNetCore.Caching.Interfaces;
using RedLockNet;

namespace QA.DotNetCore.Caching.Distributed;

public class ExternalLockFactory : ILockFactory
{
    private readonly IDistributedLockFactory _lockFactory;
    private readonly ExternalCacheSettings _settings;
    private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

    public ExternalLockFactory(IDistributedLockFactory lockFactory, ExternalCacheSettings settings)
    {
        _lockFactory = lockFactory;
        _settings = settings;
    }

    public ILock CreateLock(string key) => new ExternalLock(key, _lockFactory, _settings);

    public IAsyncLock CreateAsyncLock(string key) => new ExternalLock(key, _lockFactory, _settings);

    public void DeleteLocksOlderThan(DateTime dateTime) => _logger.Info("Deleting keys for external locks is not supported.");
}
