using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using QA.DotNetCore.Caching.Exceptions;
using QA.DotNetCore.Caching.Helpers.Operations;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching
{
    /// <summary>
    /// Реализует провайдер кеширования данных
    /// </summary>
    public partial class VersionedCacheCoreProvider : ICacheProvider, ICacheInvalidator, IMemoryCacheProvider, IDistributedMemoryCacheProvider
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
        /// Получает данные из кеша по ключам
        /// </summary>
        /// <param name="keys">Ключи</param>
        /// <returns>Данные в том же порядке что и ключи.</returns>
        public virtual IEnumerable<object> Get(IEnumerable<string> keys)
        {
            if (keys is null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            foreach (var key in keys)
            {
                yield return string.IsNullOrEmpty(key) ? null : _cache.Get(key);
            }
        }

        /// <summary>
        /// Проверяет наличие данных в кеше по ключам
        /// </summary>
        /// <param name="keys">Ключи</param>
        /// <returns>Список наличия ключей в кеше.</returns>
        public virtual IEnumerable<bool> IsSet(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                yield return !string.IsNullOrEmpty(key) && _cache.TryGetValue(key, out _);
            }
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
        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.")]
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
        /// Инвалидирует все записи в кеше по тегам
        /// </summary>
        /// <param name="tags">Теги</param>
        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.")]
        public virtual void InvalidateByTags(params string[] tags)
        {
            foreach (var tag in tags)
            {
                _cache.Remove(tag);
            }
        }

        T IMemoryCacheProvider.GetOrAdd<T>(string cacheKey, TimeSpan expiration, Func<T> getData, TimeSpan waitForCalculateTimeout) =>
            this.GetOrAdd(cacheKey, expiration, getData, waitForCalculateTimeout);

        public TResult[] GetOrAdd<TId, TResult>(
            CacheInfo<TId>[] cacheInfos,
            DataValuesFactoryDelegate<TId, TResult> dataValuesFactory,
            TimeSpan waitForCalculateTimeout = default)
        {
            IDisposable locker = null;

            try
            {
                return new OperationsChain<CacheInfo<TId>, TResult>()
                    .AddOperation(GetExistingCache)
                    .AddOperation((infos) => LockOrSetDeprecatedCache<TId, TResult>(infos, waitForCalculateTimeout, out locker))
                    .AddOperation(GetExistingCache)
                    .AddOperation((infos) => GetAndCacheRealData(infos, dataValuesFactory))
                    .Execute(cacheInfos)
                    .ToArray();
            }
            finally
            {
                locker?.Dispose();
            }

            IEnumerable<OperationResult<TResult>> GetExistingCache(CacheInfo<TId>[] infos) =>
                GetExistingCache<TId, TResult>(infos);
        }

        private IEnumerable<OperationResult<TResult>> GetExistingCache<TId, TResult>(CacheInfo<TId>[] infos)
        {
            var keys = infos.Select(info => info.Key);

            var results = Get(keys);

            foreach (var result in results)
            {
                bool isFinalResult = result != null;

                yield return new OperationResult<TResult>(Cast<TResult>(result), isFinalResult);
            }
        }

        private IEnumerable<OperationResult<TResult>> LockOrSetDeprecatedCache<TId, TResult>(
            CacheInfo<TId>[] infos,
            TimeSpan lockEnterWaitTimeout,
            out IDisposable locker)
        {
            var cacheKeys = infos.Select(info => info.Key).ToList();

            var lockersCollection = new KeyLockersCollection(_lockers, cacheKeys, Get);

            var results = new OperationResult<TResult>[infos.Length];

            lockersCollection.Lock(
                lockEnterWaitTimeout,
                (cacheKey, index, deprecatedResult) => results[index] = new OperationResult<TResult>(Cast<TResult>(deprecatedResult), true));

            locker = lockersCollection;

            return results;
        }

        private IEnumerable<TResult> GetAndCacheRealData<TId, TResult>(
            IEnumerable<CacheInfo<TId>> infosEnumerable,
            DataValuesFactoryDelegate<TId, TResult> dataValuesFactory)
        {
            var infos = infosEnumerable.ToArray();
            var realData = dataValuesFactory(infos);

            // Fill missing results with real data.
            using var resultsEnumerator = realData.GetEnumerator();

            int recievedIndex;
            for (recievedIndex = 0; resultsEnumerator.MoveNext(); recievedIndex++)
            {
                TResult currentResult = resultsEnumerator.Current;

                yield return currentResult;

                if (currentResult != null)
                {
                    var cacheInfo = infos[recievedIndex];
                    var tags = cacheInfo.Tags;
                    var expiration = cacheInfo.Expiration;
                    string missingKey = cacheInfo.Key;
                    string deprecatedCacheKey = GetDeprecatedCacheKey(missingKey);

                    //добавим новое значение в кэш и сразу обновим deprecated значение, которое хранится в 2 раза дольше, чем основное
                    Add(currentResult, missingKey, tags.ToArray(), expiration);
                    Add(currentResult, deprecatedCacheKey, Array.Empty<string>(), expiration + expiration);
                }
            }

            if (recievedIndex != infos.Length)
            {
                throw new InvalidOperationException(
                    $"Factory {nameof(dataValuesFactory)} should return the same number of elements it recieved " +
                    $"(expected: {infos.Length}, returned: {recievedIndex})");
            }
        }

        private static bool IsCacheMissing(int index, in bool[] locked)
        {
            bool isDeprecatedCacheMissing = locked[index];
            // Filter out missing key if by it had been obtained deprecated value.
            return isDeprecatedCacheMissing;
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
            var result = Cast<T>(this.Get(cacheKey));
            if (result == null)
            {
                bool lockTaken = false;
                var locker = _lockers.GetOrAdd(cacheKey, new object());
                try
                {
                    var deprecatedResult = Cast<T>(this.Get(deprecatedCacheKey));

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
                        result = Cast<T>(this.Get(cacheKey));
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

            var result = Cast<T>(this.Get(cacheKey));
            if (result == null)
            {
                SemaphoreSlim locker = _semaphores.GetOrAdd(cacheKey, _ => new SemaphoreSlim(1));
                bool lockTaken = false;

                try
                {
                    var deprecatedObjectResult = this.Get(deprecatedCacheKey);
                    var deprecatedResult = Cast<T>(deprecatedObjectResult);

                    if (deprecatedObjectResult != null)
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
                        result = Cast<T>(this.Get(cacheKey));
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

        private static T Cast<T>(object result)
        {
            return result == null ? default : (T)result;
        }
    }
}
