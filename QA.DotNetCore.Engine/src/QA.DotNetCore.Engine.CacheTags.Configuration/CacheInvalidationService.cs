using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
    public class CacheInvalidationService : IHostedService, IDisposable
    {
        private readonly ILogger<CacheInvalidationService> _logger;
        private readonly TimeSpan _interval;
        private readonly Timer _timer;
        private readonly IServiceProvider _provider;
        private readonly ICacheTagWatcher _watcher;

        public CacheInvalidationService(
            ILogger<CacheInvalidationService> logger,
            CacheTagsRegistrationConfigurator cfg,
            IServiceScopeFactory factory)
        {
            _logger = logger;
            _provider = factory.CreateScope().ServiceProvider;
            _watcher = _provider.GetRequiredService<ICacheTagWatcher>();
            _interval = cfg.TimerInterval;
            _timer = new Timer(
                OnTick,
                null,
                Timeout.InfiniteTimeSpan,
                Timeout.InfiniteTimeSpan);
        }

        private void OnTick(object? state)
        {
            _logger.LogDebug("Cache invalidation started");
            _watcher.TrackChanges(_provider);
            _logger.LogDebug("Cache invalidation completed");
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _ = _timer.Change(TimeSpan.Zero, _interval);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _ = _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
