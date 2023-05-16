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
        private readonly IServiceScopeFactory _factory;

        private readonly Timer _timer;

        public CacheInvalidationService(
            ILogger<CacheInvalidationService> logger,
            CacheTagsRegistrationConfigurator cfg,
            IServiceScopeFactory factory)
        {
            _logger = logger;
            _factory = factory;
            _interval = cfg.TimerInterval;
            _timer = new Timer(
                OnTick,
                null,
                Timeout.InfiniteTimeSpan,
                Timeout.InfiniteTimeSpan);
        }

        private void OnTick(object? state)
        {
            using var scope = _factory.CreateScope();

            _logger.LogDebug("Cache invalidation started");
            var cacheTagWatcher = scope.ServiceProvider.GetRequiredService<ICacheTagWatcher>(); 
            cacheTagWatcher.TrackChanges(scope.ServiceProvider);
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
