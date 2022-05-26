using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using QA.DotNetCore.Caching.Exceptions;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching
{
    /// <summary>
    /// Реализует провайдер кеширования данных
    /// </summary>
    public class VersionedCacheCoreProvider : ICacheProvider, ICacheInvalidator, IMemoryCacheProvider
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultWaitForCalculateTimeout = TimeSpan.FromSeconds(5);
        private static readonly ConcurrentDictionary<string, object> _lockers = new ConcurrentDictionary<string, object>();
        private static readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

        public VersionedCacheCoreProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Получает данные из кеша по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        public virtual object Get(string key)
        {
            return string.IsNullOrEmpty(key) ? null : _cache.Get(key);
        }

        /// <summary>
        /// Записывает данные в кеш
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="cacheTimeInSeconds">Время кеширования в секундах</param>
        public virtual void Set(string key, object data, int cacheTimeInSeconds)
        {
            Set(key, data, TimeSpan.FromSeconds(cacheTimeInSeconds));
        }

        /// <summary>
        /// Записывает данные в кеш
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="expiration">Время кеширования (sliding expiration)</param>
        public virtual void Set(string key, object data, TimeSpan expiration)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var policy = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now + expiration,
                Size = 1
            };

            _cache.Set(key, data, policy);
        }

        /// <summary>
        /// Проверяет наличие данных в кеше
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        public virtual bool IsSet(string key)
        {
            return !string.IsNullOrEmpty(key) && _cache.TryGetValue(key, out object result);
        }

        /// <summary>
        /// Пытается получить данные из кеша по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="result">Результат</param>
        /// <returns></returns>
        public virtual bool TryGetValue(string key, out object result)
        {
            return _cache.TryGetValue(key, out result);
        }

        /// <summary>
        /// Очищает кеш по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        public virtual void Invalidate(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            _cache.Remove(key);
            _cache.Remove(GetDeprecatedCacheKey(key));
        }

        /// <summary>
        /// Записывает данные в кеш, маркирует эту запись тегами
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="tags">Теги</param>
        /// <param name="expiration">Время кеширования (sliding expiration)</param>
        public virtual void Add(object data, string key, string[] tags, TimeSpan expiration)
        {
            var policy = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now + expiration,
                Size = 1
            };

            if (tags != null && tags.Length > 0)
            {
                var now = DateTime.Now;
                var tagExpiration = now.AddDays(10);

                foreach (var item in tags)
                {
                    var src = AddTag(tagExpiration, item);
                    policy.AddExpirationToken(new CancellationChangeToken(src.Token));
                }
            }

            _cache.Set(key, data, policy);
        }
        /// <summary>
        /// Инвалидирует все записи в кеше по тегу
        /// </summary>
        /// <param name="tag">Тег</param>
        public void InvalidateByTag(string tag)
        {
            if (String.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException("tag");
            }

            _cache.Remove(tag);
        }

        /// <summary>
        /// Инвалидирует все записи в кеше по тегам
        /// </summary>
        /// <param name="tags">Теги</param>
        public void InvalidateByTags(params string[] tags)
        {
            foreach (var tag in tags)
            {
                _cache.Remove(tag);
            }
        }

        /// <summary>
        /// Потокобезопасно берет объект из кэша, если его там нет, то вызывает функцию для получения данных
        /// и кладет результат в кэш. При устаревании кэша старый результат еще хранится какое-то время.
        /// Когда кэш устаревает, первый поток, который обратился за ним начинает вычислять новый результат, а в это время
        /// другие параллельно обратившиеся потоки будут получать устаревшее значение, если оно есть.
        /// Если устаревшего значения нет, то другие параллельные потоки будут ждать пока основной поток вычислит новый
        /// результат (они могут не дождаться этого, если истечёт waitForCalculateTimeout, то будет возвращен null).
        /// </summary>
        /// <typeparam name="T">тип объектов в кэше</typeparam>
        /// <param name="cacheKey">Ключ</param>
        /// <param name="expiration">время жизни в кэше</param>
        /// <param name="getData">функция для получения данных, если объектов кэше нет. нужно использовать анонимный делегат</param>
        /// <param name="waitForCalculateTimeout">таймаут ожидания параллельными потоками события окончания
        /// вычисления <paramref name="getData"/> по истечении  которого им будет возвращён null.
        /// Актуален только когда в кэше нет устаревшего значения. По умолчанию используется 5 секунд.</param>
        /// <exception cref="DeprecateCacheIsExpiredOrMissingException">Выбрасывается в том случае, если другой поток уже выполняет запрос
        /// на обновления данных в кеше, а старых данные ещё (или уже) нет</exception>
        public virtual T GetOrAdd<T>(string cacheKey, TimeSpan expiration, Func<T> getData,
            TimeSpan waitForCalculateTimeout = default(TimeSpan))
        {
            return GetOrAdd(cacheKey, null, expiration, getData, waitForCalculateTimeout);
        }

        /// <summary>
        /// Потокобезопасно берет объект из кэша, если его там нет, то вызывает функцию для получения данных
        /// и кладет результат в кэш, маркируя его тегами. При устаревании кэша старый результат еще хранится какое-то время.
        /// Когда кэш устаревает, первый поток, который обратился за ним начинает вычислять новый результат, а в это время
        /// другие параллельно обратившиеся потоки будут получать устаревшее значение, если оно есть.
        /// Если устаревшего значения нет, то другие параллельные потоки будут ждать пока основной поток вычислит новый
        /// результат (они могут не дождаться этого, если истечёт waitForCalculateTimeout, то будет возвращен null).
        /// </summary>
        /// <typeparam name="T">тип объектов в кэше</typeparam>
        /// <param name="cacheKey">Ключ</param>
        /// <param name="tags">Теги</param>
        /// <param name="expiration">время жизни в кэше</param>
        /// <param name="getData">функция для получения данных, если объектов кэше нет. нужно использовать анонимный делегат</param>
        /// <param name="waitForCalculateTimeout">таймаут ожидания параллельными потоками события окончания
        /// вычисления <paramref name="getData"/> по истечении  которого им будет возвращён null.
        /// Актуален только когда в кэше нет устаревшего значения. По умолчанию используется 5 секунд.</param>
        /// <exception cref="DeprecateCacheIsExpiredOrMissingException">Выбрасывается в том случае, если другой поток уже выполняет запрос
        /// на обновления данных в кеше, а старых данные ещё (или уже) нет</exception>
        public virtual T GetOrAdd<T>(
            string cacheKey,
            string[] tags,
            TimeSpan expiration,
            Func<T> getData,
            TimeSpan waitForCalculateTimeout = default(TimeSpan))
        {
            var deprecatedCacheKey = GetDeprecatedCacheKey(cacheKey);
            var result = Convert<T>(Get(cacheKey));
            if (result == null)
            {
                bool lockTaken = false;
                var locker = _lockers.GetOrAdd(cacheKey, new object());
                try
                {
                    var deprecatedResult = Convert<T>(Get(deprecatedCacheKey));

                    if (deprecatedResult != null)
                    {
                        //проверим, взял ли блокировку по этому кэшу какой-то поток (т.е. вычисляется ли уже новое значение).
                        //т.к. найдено deprecated значение, то не будем ждать освобождения блокировки, если она будет
                        //потому что нам и так есть что вернуть
                        Monitor.TryEnter(locker, ref lockTaken);
                    }
                    else
                    {
                        //проверим, взял ли блокировку по этому кэшу какой-то поток (т.е. вычисляется ли уже новое значение).
                        //если взял, то т.к. deprecated значения нет, то надо ждать освобождения блокировки, чтобы нам было что вернуть
                        //но ждать будем не дольше, чем waitForCalculateTimeout
                        if (waitForCalculateTimeout == default(TimeSpan))
                        {
                            waitForCalculateTimeout = _defaultWaitForCalculateTimeout;
                        }

                        Monitor.TryEnter(locker, (int)waitForCalculateTimeout.TotalMilliseconds, ref lockTaken);
                    }

                    if (lockTaken)
                    {
                        result = Convert<T>(Get(cacheKey));
                        if (result == null)
                        {
                            result = getData();
                            if (result != null)
                            {
                                //добавим новое значение в кэш и сразу обновим deprecated значение, которое хранится в 2 раза дольше, чем основное
                                Add(result, cacheKey, tags, expiration);
                                Add(result, deprecatedCacheKey, Array.Empty<string>(), TimeSpan.FromTicks(expiration.Ticks * 2));
                            }
                        }
                    }
                    else
                    {
                        if (deprecatedResult == null)
                        {
                            throw new DeprecateCacheIsExpiredOrMissingException();
                        }

                        result = deprecatedResult;
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        Monitor.Exit(locker);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Потокобезопасно берет объект из кэша, если его там нет, то вызывает функцию для получения данных
        /// и кладет результат в кэш. При устаревании кэша старый результат еще хранится какое-то время.
        /// Когда кэш устаревает, первый поток, который обратился за ним начинает вычислять новый результат, а в это время
        /// другие параллельно обратившиеся потоки будут получать устаревшее значение, если оно есть.
        /// Если устаревшего значения нет, то другие параллельные потоки будут ждать пока основной поток вычислит новый
        /// результат (они могут не дождаться этого, если истечёт waitForCalculateTimeout, то будет возвращен null).
        /// ВАЖНО: не поддерживается рекурсивный вызов с одинаковыми ключами (ограничение SemaphoreSlim).
        /// </summary>
        /// <typeparam name="T">тип объектов в кэше</typeparam>
        /// <param name="cacheKey">Ключ</param>
        /// <param name="expiration">время жизни в кэше</param>
        /// <param name="getData">функция для получения данных, если объектов кэше нет. нужно использовать анонимный делегат</param>
        /// <param name="waitForCalculateTimeout">таймаут ожидания параллельными потоками события окончания
        /// вычисления <paramref name="getData"/> по истечении  которого им будет возвращён null.
        /// Актуален только когда в кэше нет устаревшего значения. По умолчанию используется 5 секунд.</param>
        /// <exception cref="DeprecateCacheIsExpiredOrMissingException">Выбрасывается в том случае, если другой поток уже выполняет запрос
        /// на обновления данных в кеше, а старых данные ещё (или уже) нет</exception>
        public virtual Task<T> GetOrAddAsync<T>(string cacheKey, TimeSpan expiration,
            Func<Task<T>> getData, TimeSpan waitForCalculateTimeout = default(TimeSpan))
        {
            return GetOrAddAsync(cacheKey, null, expiration, getData, waitForCalculateTimeout);
        }

        /// <summary>
        /// Потокобезопасно берет объект из кэша, если его там нет, то вызывает функцию для получения данных
        /// и кладет результат в кэш, маркируя его тегами. При устаревании кэша старый результат еще хранится какое-то время.
        /// Когда кэш устаревает, первый поток, который обратился за ним начинает вычислять новый результат, а в это время
        /// другие параллельно обратившиеся потоки будут получать устаревшее значение, если оно есть.
        /// Если устаревшего значения нет, то другие параллельные потоки будут ждать пока основной поток вычислит новый
        /// результат (они могут не дождаться этого, если истечёт waitForCalculateTimeout, то будет возвращен null).
        /// ВАЖНО: не поддерживается рекурсивный вызов с одинаковыми ключами (ограничение SemaphoreSlim).
        /// </summary>
        /// <typeparam name="T">тип объектов в кэше</typeparam>
        /// <param name="cacheKey">Ключ</param>
        /// <param name="tags">Теги</param>
        /// <param name="expiration">время жизни в кэше</param>
        /// <param name="getData">функция для получения данных, если объектов кэше нет. нужно использовать анонимный делегат</param>
        /// <param name="waitForCalculateTimeout">таймаут ожидания параллельными потоками события окончания
        /// вычисления <paramref name="getData"/> по истечении  которого им будет возвращён null.
        /// Актуален только когда в кэше нет устаревшего значения. По умолчанию используется 5 секунд.</param>
        /// <exception cref="DeprecateCacheIsExpiredOrMissingException">Выбрасывается в том случае, если другой поток уже выполняет запрос
        /// на обновления данных в кеше, а старых данные ещё (или уже) нет</exception>
        public virtual async Task<T> GetOrAddAsync<T>(string cacheKey, string[] tags, TimeSpan expiration,
            Func<Task<T>> getData, TimeSpan waitForCalculateTimeout = default(TimeSpan))
        {
            var deprecatedCacheKey = GetDeprecatedCacheKey(cacheKey);

            var result = Convert<T>(Get(cacheKey));
            if (result == null)
            {
                SemaphoreSlim locker = _semaphores.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1));
                bool lockTaken = false;

                try
                {
                    var deprecatedResult = Convert<T>(Get(deprecatedCacheKey));

                    if (deprecatedResult != null)
                    {
                        //проверим, взял ли блокировку по этому кэшу какой-то поток (т.е. вычисляется ли уже новое значение).
                        //т.к. найдено deprecated значение, то не будем ждать освобождения блокировки, если она будет
                        //потому что нам и так есть что вернуть
                        lockTaken = await locker.WaitAsync(0)
                            .ConfigureAwait(false);
                    }
                    else
                    {
                        //проверим, взял ли блокировку по этому кэшу какой-то поток (т.е. вычисляется ли уже новое значение).
                        //если взял, то т.к. deprecated значения нет, то надо ждать освобождения блокировки, чтобы нам было что вернуть
                        //но ждать будем не дольше, чем waitForCalculateTimeout
                        if (waitForCalculateTimeout == default(TimeSpan))
                        {
                            waitForCalculateTimeout = _defaultWaitForCalculateTimeout;
                        }

                        lockTaken = await locker
                            .WaitAsync(waitForCalculateTimeout)
                            .ConfigureAwait(false);
                    }

                    if (lockTaken)
                    {
                        result = Convert<T>(Get(cacheKey));
                        if (result == null)
                        {
                            result = await getData().ConfigureAwait(false);
                            if (result != null)
                            {
                                //добавим новое значение в кэш и сразу обновим deprecated значение, которое хранится в 2 раза дольше, чем основное
                                Add(result, cacheKey, tags, expiration);
                                Add(result, deprecatedCacheKey, null, TimeSpan.FromTicks(expiration.Ticks * 2));
                            }
                        }
                    }
                    else
                    {
                        if (deprecatedResult == null)
                        {
                            throw new DeprecateCacheIsExpiredOrMissingException();
                        }

                        result = deprecatedResult;
                    }
                }
                finally
                {
                    if (lockTaken)
                    {
                        locker.Release();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Автовосстановление кеш-тега
        /// </summary>
        private static void EvictionTagCallback(object key, object value, EvictionReason reason, object state)
        {
            (value as CancellationTokenSource)?.Cancel();

            var strkey = key as string;
            if (strkey != null)
            {
                ((VersionedCacheCoreProvider)state).AddTag(DateTime.Now.AddDays(1), strkey);
            }
        }

        private CancellationTokenSource AddTag(DateTime tagExpiration, string item)
        {
            var result = _cache.Get(item) as CancellationTokenSource;
            if (result == null)
            {
                result = new CancellationTokenSource();
                var options = new MemoryCacheEntryOptions()
                {
                    Priority = CacheItemPriority.NeverRemove,
                    AbsoluteExpiration = tagExpiration,
                    Size = 1
                };
                options.RegisterPostEvictionCallback(EvictionTagCallback, this);
                _cache.Set(item, result, options);
            }
            return result;
        }

        private static string GetDeprecatedCacheKey(string originalKey)
        {
            return originalKey + "__Deprecated";
        }

        private static T Convert<T>(object result)
        {
            return result == null ? default(T) : (T)result;
        }
    }
}
