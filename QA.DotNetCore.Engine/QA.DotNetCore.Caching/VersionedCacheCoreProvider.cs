using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace QA.DotNetCore.Caching
{
    /// <summary>
    /// Реализует провайдер кеширования данных
    /// </summary>
    public class VersionedCacheCoreProvider : ICacheProvider
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _defaultMonitorTimeout = TimeSpan.FromSeconds(5);
        private static ConcurrentDictionary<string, object> _lockers = new ConcurrentDictionary<string, object>();

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
            return _cache.Get(key);
        }

        /// <summary>
        /// Записывает данные в кеш
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="cacheTime">Время кеширования в секундах</param>
        public virtual void Set(string key, object data, int cacheTime)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }

            var policy = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now + TimeSpan.FromSeconds(cacheTime)
            };

            _cache.Set(key, data, policy);
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
                AbsoluteExpiration = DateTime.Now + expiration
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

        public virtual bool TryGetValue(string key, out object result)
        {
            return _cache.TryGetValue(key, out result);
        }

        public virtual T GetOrAdd<T>(string cacheKey, TimeSpan expiration, Func<T> getData, TimeSpan monitorTimeout = default(TimeSpan))
        {
            var deprecatedCacheKey = GetDeprecatedCacheKey(cacheKey);
            var result = Convert<T>(Get(cacheKey));
            if (result == null)
            {
                bool refreshing = false;
                var _locker = _lockers.GetOrAdd(cacheKey, new object());
                try
                {
                    var deprecatedResult = Convert<T>(Get(deprecatedCacheKey));

                    if (deprecatedResult != null)
                    {
                        // если есть старое значение, то обновлять данные будет только 1 поток
                        Monitor.TryEnter(_locker, ref refreshing);
                    }
                    else
                    {
                        //если старого значения нет, то надо ждать
                        if (monitorTimeout == default(TimeSpan))
                            monitorTimeout = _defaultMonitorTimeout;

                        Monitor.TryEnter(_locker, (int)monitorTimeout.TotalMilliseconds, ref refreshing);
                    }

                    if (refreshing)
                    {
                        //выполняет 1 поток
                        result = Convert<T>(Get(cacheKey));
                        if (result == null)
                        {
                            result = getData();
                            if (result != null)
                            {
                                Set(cacheKey, result, expiration);
                                Set(deprecatedCacheKey, result, TimeSpan.FromTicks(expiration.Ticks * 2));
                            }
                        }
                    }
                    else
                    {
                        result = deprecatedResult;
                        //if (deprecatedResult == null)
                        //    throw new Exception($"GetOrAdd Exception. deprecatedResult is null and thread did not wait for result. Timeout is {(int)monitorTimeout.TotalMilliseconds} ms.");
                    }
                }
                finally
                {
                    if (refreshing)
                    {
                        Monitor.Exit(_locker);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Очищает кеш
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

        public void Add(object data, string key, string[] tags, TimeSpan expiration)
        {
            var policy = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now + expiration
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


        public void InvalidateByTag(string tag)
        {
            if (String.IsNullOrEmpty(tag))
                throw new ArgumentNullException("tag");

            _cache.Remove(tag);
        }

        public void InvalidateByTags(params string[] tags)
        {
            foreach (var tag in tags)
            {
                _cache.Remove(tag);
            }
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
            return _cache.GetOrCreate(item, entry =>
            {
                entry.Priority = CacheItemPriority.NeverRemove;
                entry.AbsoluteExpiration = tagExpiration;
                entry.RegisterPostEvictionCallback(callback: EvictionTagCallback, state: this);
                return new CancellationTokenSource();
            });
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
