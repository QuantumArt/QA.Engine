using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using QA.DotNetCore.Caching.Exceptions;
using QA.DotNetCore.Caching.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching
{
    /// <summary>
    /// Реализует провайдер кеширования данных
    /// </summary>
    public class VersionedCacheCoreProvider : ICacheProvider, ICacheInvalidator, IMemoryCacheProvider
    {
        protected readonly IMemoryCache _cache;
        protected readonly ICacheKeyFactory _keyFactory;
        protected readonly ILockFactory _lockFactory;
        protected readonly TimeSpan _defaultWaitForCalculateTimeout = TimeSpan.FromSeconds(5);
        protected readonly int _defaultDeprecatedCoef = 2;
        private readonly ILogger _logger;

        public VersionedCacheCoreProvider(
            IMemoryCache cache,
            ICacheKeyFactory keyFactory,
            ILockFactory lockFactory,
            ILogger logger
        )
        {
            _cache = cache;
            _logger = logger;
            _lockFactory = lockFactory;
            _keyFactory = keyFactory;
        }

        public VersionedCacheCoreProvider(IMemoryCache cache, ICacheKeyFactory keyFactory, ILockFactory lockFactory,
            ILogger<VersionedCacheCoreProvider> genericLogger)
            : this(cache, keyFactory, lockFactory, logger: genericLogger)
        {
        }

        /// <summary>
        /// Получает данные из кеша по ключам
        /// </summary>
        /// <param name="keys">Ключи</param>
        /// <returns>Данные в том же порядке что и ключи.</returns>
        public virtual IEnumerable<TResult> Get<TResult>(IEnumerable<string> keys)
        {
            if (keys is null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            foreach (var key in keys)
            {
                yield return string.IsNullOrEmpty(key) ? default : _cache.Get<TResult>(GetKey(key));
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
                yield return !string.IsNullOrEmpty(key) && _cache.TryGetValue(GetKey(key), out _);
            }
        }
        
        
        public void Set(string key, object data, TimeSpan expiration)
        {
            Add(data, key, Array.Empty<string>(), expiration);
        }


        /// <summary>
        /// Пытается получить данные из кеша по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="result">Результат</param>
        /// <returns></returns>
        public virtual bool TryGetValue<TResult>(string key, out TResult result)
        {
            return _cache.TryGetValue(GetKey(key), out result);
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

            key = GetKey(key);

            _cache.Remove(key);
        }

        /// <inheritdoc/>
        public virtual void Add(object data, string key, string[] tags, TimeSpan expiration, bool skipSerialization = false)
        {
            key = GetKey(key);

            //добавим новое значение в кеш и сразу обновим deprecated значение, которое хранится в 2 раза дольше, чем основное
            var deprecatedKey = GetDeprecatedKey(key);
            var deprecatedExpiration = expiration * _defaultDeprecatedCoef;

            var policy = GetPolicy(expiration);
            var deprecatedPolicy = GetPolicy(deprecatedExpiration);

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
            _cache.Set(deprecatedKey, data, deprecatedPolicy);
        }


        private static MemoryCacheEntryOptions GetPolicy(TimeSpan expiration)
        {
            var policy = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now + expiration,
                Size = 1
            };
            return policy;
        }

        /// <summary>
        /// Инвалидирует записи в кеше по тегам
        /// </summary>
        /// <param name="tags">Теги</param>
        public virtual void InvalidateByTags(params string[] tags)
        {
            foreach (var tag in tags)
            {
                _cache.Remove(GetTag(tag));
            }
        }

        /// <summary>
        /// Проверяет наличие данных в кэше
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns>Присутствует ли ключ к кэше</returns>
        public bool IsSet(string key)
        {
            var results = IsSet(new[] {key});
            return results.Single();
        }

        /// <summary>
        /// Получает данные из кэша по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        public TResult Get<TResult>(string key)
        {
            var results = Get<TResult>(new[] {key});
            return results.Single();
        }

        T IMemoryCacheProvider.GetOrAdd<T>(string cacheKey, TimeSpan expiration, Func<T> getData,
            TimeSpan waitForCalculateTimeout) =>
            this.GetOrAdd(cacheKey, expiration, getData, waitForCalculateTimeout);

        public Task<T> GetOrAddAsync<T>(string cacheKey, TimeSpan expiration, Func<Task<T>> getData,
            TimeSpan waitForCalculateTimeout = default) =>
            GetOrAddAsync(cacheKey, Array.Empty<string>(), expiration, getData, waitForCalculateTimeout);

        public T GetOrAdd<T>(string cacheKey, TimeSpan expiration, Func<T> getData,
            TimeSpan waitForCalculateTimeout = default) =>
            GetOrAdd(cacheKey, Array.Empty<string>(), expiration, getData, waitForCalculateTimeout);

        /// <summary>
        /// Потокобезопасно берет объект из кеша, если его там нет, то вызывает функцию для получения данных
        /// и кладет результат в кеш, маркируя его тегами. При устаревании кеша старый результат еще хранится какое-то время.
        /// Когда кеш устаревает, первый поток, который обратился за ним начинает вычислять новый результат, а в это время
        /// другие параллельно обратившиеся потоки будут получать устаревшее значение, если оно есть.
        /// Если устаревшего значения нет, то другие параллельные потоки будут ждать пока основной поток вычислит новый
        /// результат (они могут не дождаться этого, если истечёт waitForCalculateTimeout, то будет возвращен null).
        /// </summary>
        /// <typeparam name="T">тип объектов в кеше</typeparam>
        /// <param name="cacheKey">Ключ</param>
        /// <param name="tags">Теги</param>
        /// <param name="expiration">время жизни в кеше</param>
        /// <param name="getData">функция для получения данных, если объектов кеше нет. нужно использовать анонимный делегат</param>
        /// <param name="waitForCalculateTimeout">таймаут ожидания параллельными потоками события окончания
        /// вычисления <paramref name="getData"/> по истечении  которого им будет возвращён null.
        /// Актуален только когда в кеше нет устаревшего значения. По умолчанию используется 5 секунд.</param>
        /// <param name="skipSerialization"></param>
        /// <exception cref="DeprecateCacheIsExpiredOrMissingException">Выбрасывается в том случае, если другой поток уже выполняет запрос
        /// на обновления данных в кеше, а старых данные ещё (или уже) нет</exception>
        public virtual T GetOrAdd<T>(
            string cacheKey,
            string[] tags,
            TimeSpan expiration,
            Func<T> getData,
            TimeSpan waitForCalculateTimeout = default,
            bool skipSerialization = false)
        {
            cacheKey = GetKey(cacheKey);

            var deprecatedCacheKey = GetDeprecatedKey(cacheKey);
            var result = this.Get<T>(cacheKey);
            if (result == null)
            {
                bool lockTaken = false;
                var locker = _lockFactory.CreateLock(cacheKey);
                try
                {
                    //проверим, взял ли блокировку по этому кешу какой-то поток (т.е. вычисляется ли уже новое значение).
                    //если взял, то и deprecated-значение, чтобы вернуть сразу, отсутствует, то надо ждать освобождения блокировки, чтобы нам было что вернуть
                    //но ждать будем не дольше, чем waitForCalculateTimeout
                    lockTaken = locker.Acquire();
                    T deprecatedResult = default;

                    if (!lockTaken)
                    {
                        deprecatedResult = this.Get<T>(deprecatedCacheKey);
                        if (deprecatedResult == null)
                        {
                            if (waitForCalculateTimeout == default)
                            {
                                waitForCalculateTimeout = _defaultWaitForCalculateTimeout;
                            }

                            lockTaken = locker.Acquire(waitForCalculateTimeout);
                        }
                    }

                    if (lockTaken)
                    {
                        result = this.Get<T>(cacheKey);
                        if (result == null)
                        {
                            result = getData();
                            if (result != null)
                            {
                                Add(result, cacheKey, tags, expiration, skipSerialization);
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
        /// Потокобезопасно берет объект из кеша, если его там нет, то вызывает функцию для получения данных
        /// и кладет результат в кеш, маркируя его тегами. При устаревании кеша старый результат еще хранится какое-то время.
        /// Когда кеш устаревает, первый поток, который обратился за ним начинает вычислять новый результат, а в это время
        /// другие параллельно обратившиеся потоки будут получать устаревшее значение, если оно есть.
        /// Если устаревшего значения нет, то другие параллельные потоки будут ждать пока основной поток вычислит новый
        /// результат (они могут не дождаться этого, если истечёт waitForCalculateTimeout, то будет возвращен null).
        /// ВАЖНО: не поддерживается рекурсивный вызов с одинаковыми ключами (ограничение SemaphoreSlim).
        /// </summary>
        /// <typeparam name="T">тип объектов в кеше</typeparam>
        /// <param name="cacheKey">Ключ</param>
        /// <param name="tags">Теги</param>
        /// <param name="expiration">время жизни в кеше</param>
        /// <param name="getData">функция для получения данных, если объектов кеше нет. нужно использовать анонимный делегат</param>
        /// <param name="waitForCalculateTimeout">таймаут ожидания параллельными потоками события окончания
        /// вычисления <paramref name="getData"/> по истечении  которого им будет возвращён null.
        /// Актуален только когда в кеше нет устаревшего значения. По умолчанию используется 5 секунд.</param>
        /// <param name="skipSerialization"></param>
        /// <exception cref="DeprecateCacheIsExpiredOrMissingException">Выбрасывается в том случае, если другой поток уже выполняет запрос
        /// на обновления данных в кеше, а старых данные ещё (или уже) нет</exception>
        public virtual async Task<T> GetOrAddAsync<T>(
            string cacheKey,
            string[] tags,
            TimeSpan expiration,
            Func<Task<T>> getData,
            TimeSpan waitForCalculateTimeout = default,
            bool skipSerialization = false)
        {
            cacheKey = GetKey(cacheKey);

            var deprecatedCacheKey = GetDeprecatedKey(cacheKey);
            var result = this.Get<T>(cacheKey);
            if (result == null)
            {
                T deprecatedResult = default;
                var locker = _lockFactory.CreateAsyncLock(cacheKey);
                bool lockTaken = false;

                try
                {
                    //проверим, взял ли блокировку по этому кешу какой-то поток (т.е. вычисляется ли уже новое значение).
                    //если взял и deprecated-значение, чтобы вернуть сразу, отсутствует, то надо ждать освобождения блокировки, чтобы нам было что вернуть
                    //но ждать будем не дольше, чем waitForCalculateTimeout
                    lockTaken = await locker.AcquireAsync().ConfigureAwait(false);

                    if (!lockTaken)
                    {
                        deprecatedResult = this.Get<T>(deprecatedCacheKey);
                        if (deprecatedResult == null)
                        {
                            if (waitForCalculateTimeout == default)
                            {
                                waitForCalculateTimeout = _defaultWaitForCalculateTimeout;
                            }

                            lockTaken = await locker.AcquireAsync(waitForCalculateTimeout).ConfigureAwait(false);
                        }
                    }


                    if (lockTaken)
                    {
                        result = this.Get<T>(cacheKey);
                        if (result == null)
                        {
                            result = await getData().ConfigureAwait(false);
                            if (result != null)
                            {
                                Add(result, cacheKey, tags, expiration, skipSerialization);
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
                        await locker.ReleaseAsync();
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

            if (key is string strkey)
            {
                ((VersionedCacheCoreProvider) state).AddTag(DateTime.Now.AddDays(1), strkey);
            }
        }

        private CancellationTokenSource AddTag(DateTime tagExpiration, string item)
        {
            item = GetTag(item);

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

        protected string GetDeprecatedKey(string key) => _keyFactory.GetDeprecatedKey(key ?? "");

        protected string GetKey(string key) => _keyFactory.GetDataKey(key ?? "");
        protected string GetTag(string tag) => _keyFactory.GetTagKey(tag ?? "");
    }
}
