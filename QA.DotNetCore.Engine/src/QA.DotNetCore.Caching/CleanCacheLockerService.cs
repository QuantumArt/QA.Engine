using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NLog;
using QA.DotNetCore.Caching.Interfaces;

namespace QA.DotNetCore.Caching
{
    public class CleanCacheLockerService : IHostedService, IDisposable
    {
        private readonly ILockFactory _lockFactory;
        private readonly TimeSpan _runInterval;
        private readonly TimeSpan _cleanInterval;

        private readonly Timer _timer;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public CleanCacheLockerService(
            ILockFactory lockFactory,
            CleanCacheLockerServiceSettings settings
            )
        {
            _lockFactory = lockFactory;
            _runInterval = settings.RunInterval;
            _cleanInterval = settings.CleanInterval;
            _timer = new Timer(
                OnTick,
                null,
                Timeout.InfiniteTimeSpan,
                Timeout.InfiniteTimeSpan);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_runInterval != default)
            {
                _ = _timer.Change(TimeSpan.Zero, _runInterval);
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _ = _timer.Change(Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();

        private void OnTick(object _)
        {
            if (_cleanInterval != default)
            {
                var timeToDelete = DateTime.Now - _cleanInterval;
                _logger.Trace($"Cache locker cleaning older than {timeToDelete} started.");
                _lockFactory.DeleteLocksOlderThan(timeToDelete);
                _logger.Trace("Cache locker cleaning completed.");
            }
        }
    }
}
