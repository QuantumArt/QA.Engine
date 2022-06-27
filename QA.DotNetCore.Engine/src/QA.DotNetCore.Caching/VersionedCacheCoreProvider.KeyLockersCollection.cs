using QA.DotNetCore.Caching.Exceptions;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace QA.DotNetCore.Caching
{
    public partial class VersionedCacheCoreProvider
    {
        private class KeyLockersCollection : IDisposable
        {
            private readonly bool[] _lockTaken;
            private readonly object[] _lockers;
            private readonly IReadOnlyList<string> _keys;
            private readonly Func<IEnumerable<string>, IEnumerable<object>> _getCache;
            private readonly ConcurrentDictionary<string, object> _lockersRepository;

            private bool _disposedValue;

            public KeyLockersCollection(
                in ConcurrentDictionary<string, object> lockersRepository,
                in List<string> keys,
                Func<IEnumerable<string>, IEnumerable<object>> getCache)
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
                _lockTaken = new bool[keys.Count];
                _lockers = new object[keys.Count];
                _getCache = getCache ?? throw new ArgumentNullException(nameof(getCache));
                _lockersRepository = lockersRepository ?? throw new ArgumentNullException(nameof(lockersRepository));
            }

            /// <summary>
            /// Lock cache
            /// </summary>
            /// <param name="deprecatedResultHandler">Delegate to handle deprecated value when it exists.</param>
            /// <param name="lockEnterWaitTimeout">Timeout to wait entering lock when there is no deprecated value for cache.</param>
            /// <remarks>
            /// There can be 3 results:
            /// <list type="bullet">
            /// <item>
            ///     <term>Cache lock aqured successfully</term>
            ///     <description>Sets associated <see cref="Locked"/> flag to true</description>
            /// </item>
            /// <item>
            ///     <term>Cache with deprecated value not locked</term>
            ///     <description>Execute deprecated value handler</description>
            /// </item>
            /// <item>
            ///     <term>Cache without deprecated value not locked</term>
            ///     <description>Release all locks and throws <see cref="DeprecateCacheIsExpiredOrMissingException"/> exception.</description>
            /// </item>
            /// </list>
            /// </remarks>
            /// <exception cref="DeprecateCacheIsExpiredOrMissingException">
            /// Fired when waiting on locked cache without deprecated value longer then <paramref name="lockEnterWaitTimeout"/>
            /// </exception>
            public void Lock(
                in TimeSpan lockEnterWaitTimeout,
                Action<string, int, object> deprecatedResultHandler)
            {
                try
                {
                    object[] deprecatedValues = _getCache(_keys.Select(GetDeprecatedCacheKey)).ToArray();

                    for (int i = 0; i < _keys.Count; i++)
                    {
                        string cacheKey = _keys[i];
                        object deprecatedValue = deprecatedValues[i];

                        _lockers[i] = _lockersRepository.GetOrAdd(cacheKey, new object());

                        if (deprecatedValue != null)
                        {
                            //проверим, взял ли блокировку по этому кэшу какой-то поток (т.е. вычисляется ли уже новое значение).
                            //т.к. найдено deprecated значение, то не будем ждать освобождения блокировки, если она будет
                            //потому что нам и так есть что вернуть
                            Monitor.TryEnter(_lockers[i], ref _lockTaken[i]);

                            if (!_lockTaken[i])
                            {
                                deprecatedResultHandler(cacheKey, i, deprecatedValue);
                            }
                        }
                        else
                        {
                            //проверим, взял ли блокировку по этому кэшу какой-то поток (т.е. вычисляется ли уже новое значение).
                            //если взял, то т.к. deprecated значения нет, то надо ждать освобождения блокировки, чтобы нам было что вернуть
                            //но ждать будем не дольше, чем lockEnterWaitTimeout
                            Monitor.TryEnter(_lockers[i], (int)lockEnterWaitTimeout.TotalMilliseconds, ref _lockTaken[i]);

                            if (!_lockTaken[i])
                            {
                                throw new DeprecateCacheIsExpiredOrMissingException();
                            }
                        }
                    }
                }
                catch
                {
                    Unlock();
                    throw;
                }
            }

            public ReadOnlySpan<bool> Locked => _lockTaken;

            private void Unlock()
            {
                for (int i = _keys.Count - 1; i >= 0; i--)
                {
                    if (_lockTaken[i])
                    {
                        Monitor.Exit(_lockers[i]);
                        _lockTaken[i] = false;
                    }
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

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
