using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Engine.CacheTags.Configuration
{
    /// <summary>
    /// Фоновый процесс, отслеживающий изменения кештегов
    /// </summary>
    public class CacheInvalidationService : IHostedService, IDisposable
    {
        private static readonly object _locker = new();
        private bool _lockTaken;
        private readonly ILogger _logger;
        private readonly TimeSpan _interval;
        private readonly Timer _timer;
        private readonly IServiceScopeFactory _factory;

        public CacheInvalidationService(
            CacheTagsRegistrationConfigurator cfg,
            IServiceScopeFactory factory,
            ILogger<CacheInvalidationService> logger)
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
            try
            {
                Monitor.TryEnter(_locker, 0, ref _lockTaken);
                if (!_lockTaken)
                {
                    _logger.LogInformation("A previous invalidation is in progress now. Proceeding exit");
                }
                else
                {
                    _logger.LogInformation("Creating new scope");
                    using var scope = _factory.CreateScope();
                    var provider = scope.ServiceProvider;
                    var watcher = provider.GetRequiredService<ICacheTagWatcher>();
                    watcher.TrackChanges();
                }
            }
            finally
            {
                if (_lockTaken) {
                    Monitor.Exit(_locker);
                }
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _ = _timer.Change(_interval, _interval);
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
