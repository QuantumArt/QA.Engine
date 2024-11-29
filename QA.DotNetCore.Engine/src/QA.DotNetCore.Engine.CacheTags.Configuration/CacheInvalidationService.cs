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
        private readonly TimeSpan _interval;
        private readonly Timer _timer;
        private readonly IServiceProvider _provider;

        public CacheInvalidationService(
            CacheTagsRegistrationConfigurator cfg,
            IServiceScopeFactory factory)
        {
            _provider = factory.CreateScope().ServiceProvider;
            _interval = cfg.TimerInterval;
            _timer = new Timer(
                OnTick,
                null,
                Timeout.InfiniteTimeSpan,
                Timeout.InfiniteTimeSpan);
        }

        private void OnTick(object? state)
        {
            _logger.Info("Cache invalidation started");
            var watcher = _provider.GetRequiredService<ICacheTagWatcher>();
            watcher.TrackChanges(_provider);
            _logger.Info("Cache invalidation completed");
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
