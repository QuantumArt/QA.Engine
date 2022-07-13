using Microsoft.Extensions.Logging;
using QA.DotNetCore.Caching.Exceptions;
using QA.DotNetCore.Caching.Helpers.Operations;
using RedLockNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Distributed
{
    public partial class RedisCache
    {
        private class KeyLockersCollection : IDisposable, IAsyncDisposable
        {
            private readonly IRedLock[] _lockers;
            private readonly IReadOnlyList<string> _keys;
            private readonly IDistributedLockFactory _distributedLockFactory;
            private readonly TimeSpan _lockExpiration;
            private readonly TimeSpan _retryEnterLockInverval;
            private readonly ILogger _logger;
            private bool _disposedValue;

            public KeyLockersCollection(
                in IDistributedLockFactory distributedLockFactory,
                in List<string> keys,
                TimeSpan lockExpiration,
                TimeSpan retryEnterLockInverval,
                ILogger logger)
            {
                if (keys is null)
                {
                    throw new ArgumentNullException(nameof(keys));
                }

                if (keys.Count <= 0)
                {
                    throw new ArgumentException("Keys collection is empty.", nameof(keys));
                }

                // Sort to avoid deadlocks on locking.
                keys.Sort(StringComparer.OrdinalIgnoreCase);

                _keys = keys;
                _lockExpiration = lockExpiration;
                _retryEnterLockInverval = retryEnterLockInverval;
                _logger = logger;
                _lockers = new IRedLock[keys.Count];
                _distributedLockFactory = distributedLockFactory ?? throw new ArgumentNullException(nameof(distributedLockFactory));
            }

            /// <summary>
            /// Lock cache
            /// </summary>
            /// <param name="lockEnterWaitTimeout">Timeout to wait entering lock when there is no deprecated value for cache.</param>
            /// <remarks>
            /// Successfull lock or deprecated cache.
            /// </remarks>
            /// <exception cref="DeprecateCacheIsExpiredOrMissingException">
            /// Fired when waiting on locked cache without deprecated value longer than <paramref name="lockEnterWaitTimeout"/>
            /// </exception>
            public IEnumerable<OperationResult<CachedValue>> Lock(
                TimeSpan lockEnterWaitTimeout,
                OperationContext<CachedValue> context)
            {
                ValidateLockEnterTimeout(lockEnterWaitTimeout);

                try
                {
                    var results = new OperationResult<CachedValue>[_keys.Count];

                    for (int i = 0; i < _keys.Count; i++)
                    {
                        string cacheKey = _keys[i];
                        CachedValue previousValue = context.GetPreviousResult(i);

                        if (previousValue.State is KeyState.Deprecated or KeyState.Exist)
                        {
                            // Производит попытку заблокировать ключ. Если не удалось, то возвращаем deprecated значение
                            // (в случае, если кто-то уже производит эксклюзивную операцию над ключом).
                            _lockers[i] = _distributedLockFactory.CreateLock(
                                cacheKey,
                                _lockExpiration);

                            results[i] = new OperationResult<CachedValue>(previousValue, !IsAcquired(_lockers[i]));
                        }
                        else
                        {
                            var stopwatch = Stopwatch.StartNew();

                            // Ждем освобождения блокировки, т.к. deprecated значения нет
                            // (в случае, если кто-то уже производит эксклюзивную операцию над ключом).
                            _lockers[i] = _distributedLockFactory.CreateLock(
                                cacheKey,
                                _lockExpiration,
                                _retryEnterLockInverval,
                                lockEnterWaitTimeout);

                            ThrowIfNotLocked(cacheKey, _lockers[i], lockEnterWaitTimeout, stopwatch);

                            results[i] = new OperationResult<CachedValue>(previousValue, false);
                        }
                    }

                    LogLockingStatus();

                    return results;
                }
                catch
                {
                    Unlock();
                    throw;
                }
            }

            /// <summary>
            /// Lock cache
            /// </summary>
            /// <param name="lockEnterWaitTimeout">Timeout to wait entering lock when there is no deprecated value for cache.</param>
            /// <remarks>
            /// Successfull lock or deprecated cache.
            /// </remarks>
            /// <exception cref="DeprecateCacheIsExpiredOrMissingException">
            /// Fired when waiting on locked cache without deprecated value longer than <paramref name="lockEnterWaitTimeout"/>
            /// </exception>
            public async Task<IEnumerable<OperationResult<CachedValue>>> LockAsync(
                TimeSpan lockEnterWaitTimeout,
                OperationContext<CachedValue> context)
            {
                ValidateLockEnterTimeout(lockEnterWaitTimeout);

                try
                {
                    var lockers = new OperationResult<CachedValue>[_keys.Count];

                    for (int i = 0; i < _keys.Count; i++)
                    {
                        lockers[i] = await LockKeyAsync(i, lockEnterWaitTimeout, context);
                    }

                    LogLockingStatus();

                    return lockers;
                }
                catch
                {
                    Unlock();
                    throw;
                }
            }

            private async Task<OperationResult<CachedValue>> LockKeyAsync(
                int index,
                TimeSpan lockEnterWaitTimeout,
                OperationContext<CachedValue> context)
            {
                string cacheKey = _keys[index];
                CachedValue previousValue = context.GetPreviousResult(index);

                if (previousValue.State is KeyState.Deprecated or KeyState.Exist)
                {
                    // Производит попытку заблокировать ключ. Если не удалось, то возвращаем deprecated значение
                    // (в случае, если кто-то уже производит эксклюзивную операцию над ключом).
                    _lockers[index] = await _distributedLockFactory.CreateLockAsync(
                        cacheKey,
                        _lockExpiration);

                    return new OperationResult<CachedValue>(previousValue, !IsAcquired(_lockers[index]));
                }
                else
                {
                    var stopwatch = Stopwatch.StartNew();

                    // Ждем освобождения блокировки, т.к. deprecated значения нет
                    // (в случае, если кто-то уже производит эксклюзивную операцию над ключом).
                    _lockers[index] = await _distributedLockFactory.CreateLockAsync(
                        cacheKey,
                        _lockExpiration,
                        _retryEnterLockInverval,
                        lockEnterWaitTimeout);

                    ThrowIfNotLocked(cacheKey, _lockers[index], lockEnterWaitTimeout, stopwatch);

                    return new OperationResult<CachedValue>(previousValue, false);
                }
            }

            private static void ValidateLockEnterTimeout(TimeSpan lockEnterWaitTimeout)
            {
                if (lockEnterWaitTimeout <= TimeSpan.Zero)
                {
                    throw new ArgumentException(
                        "Lock enter timeout should be greated than 0.",
                        nameof(lockEnterWaitTimeout));
                }
            }

            private void LogLockingStatus()
            {
                if (_logger.IsEnabled(LogLevel.Trace))
                {
                    var lockedKeys = _lockers
                        .Select((locker, index) => (IsAcquired: IsAcquired(locker), Key: _keys[index]))
                        .Where(key => key.IsAcquired)
                        .Select(key => key.Key);

                    if (lockedKeys.Any())
                    {
                        _logger.LogInformation("Keys ({CacheKeys}) are locked", lockedKeys);
                    }
                }
            }

            private void ThrowIfNotLocked(string cacheKey, IRedLock locker, TimeSpan lockEnterWaitTimeout, Stopwatch stopwatch)
            {
                stopwatch.Stop();

                if (!IsAcquired(locker))
                {
                    throw new DeprecateCacheIsExpiredOrMissingException($"Unable to acquire lock for key '{cacheKey}'");
                }

                _logger.LogTrace(
                    "Lock for key {CacheKeys} acquired (time: {Elapled}, retryInterval: {RetryInterval}, timeout: {LockTimeout})",
                    cacheKey,
                    stopwatch.Elapsed,
                    _retryEnterLockInverval,
                    lockEnterWaitTimeout);
            }

            private static bool IsAcquired(IRedLock isAcquired) =>
                isAcquired?.IsAcquired ?? false;

            private void Unlock()
            {
                for (int i = _keys.Count - 1; i >= 0; i--)
                {
                    _logger.LogTrace("Unlocking the key {CacheKey}", _keys[i]);
                    _lockers[i]?.Dispose();
                }
            }

            private async ValueTask UnlockAsync()
            {
                for (int i = _keys.Count - 1; i >= 0; i--)
                {
                    _logger.LogTrace("Unlocking the key {CacheKey}", _keys[i]);
                    if (_lockers[i] is not null)
                    {
                        await _lockers[i].DisposeAsync();
                    }
                }
            }

            public async ValueTask DisposeAsync()
            {
                await InnerDisposeAsync();
                Dispose(disposing: false);
                GC.SuppressFinalize(this);
            }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }

            protected virtual async ValueTask InnerDisposeAsync()
            {
                if (!_disposedValue)
                {
                    await UnlockAsync();

                    _disposedValue = true;
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposedValue)
                {
                    if (disposing)
                    {
                        Unlock();
                    }

                    _disposedValue = true;
                }
            }
        }
    }
}
