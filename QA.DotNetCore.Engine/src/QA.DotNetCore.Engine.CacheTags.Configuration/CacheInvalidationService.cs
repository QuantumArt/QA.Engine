using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Engine.CacheTags.Configuration
{
    /// <summary>
    /// Фоновый процесс, отслеживающий изменения кештегов
    /// </summary>
    public class CacheInvalidationService : IHostedService, IDisposable
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly object _locker = new();
        private static bool _busy;
        private readonly TimeSpan _interval;
        private readonly Timer _timer;
        private readonly IServiceScopeFactory _factory;

        public CacheInvalidationService(
            CacheTagsRegistrationConfigurator cfg,
            IServiceScopeFactory factory)
        {
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
            if (_busy)
            {
                _logger.Info("A previous cache invalidation is in progress now. Proceeding exit");
                return;
            }

            lock (_locker)
            {
                _busy = true;
                _logger.Info("Creating new scope");
                using var scope = _factory.CreateScope();
                var provider = scope.ServiceProvider;
                _logger.Info("Cache invalidation started");
                var watcher = provider.GetRequiredService<ICacheTagWatcher>();
                watcher.TrackChanges();
                _logger.Info("Cache invalidation completed");
                _busy = false;
            }
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

        public void Dispose() => _timer.Dispose();
    }
}
