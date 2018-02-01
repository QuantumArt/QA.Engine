using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.CacheTags.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.CacheTags
{
    /// <summary>
    /// Фоновый процесс, отслеживающий изменения кештегов
    /// </summary>
    public class CacheInvalidationBackgroundService : BackgroundService
    {
        private readonly ICacheTagWatcher _cacheTagWatcher;
        private readonly ILogger<CacheInvalidationBackgroundService> _logger;
        private readonly TimeSpan _interval;

        public CacheInvalidationBackgroundService(ICacheTagWatcher cacheTagWatcher, ILogger<CacheInvalidationBackgroundService> logger, CacheTagsRegistrationConfigurator cfg)
        {
            _cacheTagWatcher = cacheTagWatcher;
            _logger = logger;
            _interval = cfg.TimerInterval;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug($"CacheInvalidationBackgroundService is starting.");

            stoppingToken.Register(() =>
                    _logger.LogDebug($"CacheInvalidation task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"CacheInvalidation task doing background work.");

                _cacheTagWatcher.TrackChanges();

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogDebug($"CacheInvalidation task is stopping.");
        }
    }
}
