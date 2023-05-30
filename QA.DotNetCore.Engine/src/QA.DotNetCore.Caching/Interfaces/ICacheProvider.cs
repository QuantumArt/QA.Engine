using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Interfaces
{
    public delegate IEnumerable<TValue> DataValuesFactoryDelegate<TId, TValue>(
        IEnumerable<CacheInfo<TId>> missingCacheInfos);

    public delegate Task<IEnumerable<TValue>> AsyncDataValuesFactoryDelegate<TId, TValue>(
        IEnumerable<CacheInfo<TId>> missingCacheInfos);

    /// <summary>
    /// General-purpose cache provider interface. Supports only serializable objects.
    /// </summary>
    public interface ICacheProvider : ICacheInvalidator
    {
        IEnumerable<TResult> Get<TResult>(IEnumerable<string> keys);

        TResult Get<TResult>(string key);
        
        bool TryGetValue<TResult>(string key, out TResult result);

        /// <summary>
        /// Проверяет наличие данных в кэше
        /// </summary>
        /// <param name="keys">Ключи</param>
        /// <returns>Список наличия ключей к кэше</returns>
        IEnumerable<bool> IsSet(IEnumerable<string> keys);

        bool IsSet(string key);

        /// <summary>
        /// Записывает данные в кеш, маркирует эту запись тегами
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="tags">Теги</param>
        /// <param name="expiration">Время кеширования (absolute expiration).</param>
        /// <param name="skipSerialization"></param>
        void Add(object data, string key, string[] tags, TimeSpan expiration, bool skipSerialization = false);

        /// <summary>
        /// Записывает данные в кеш.
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="expiration">Интервал кэширования</param>
        void Set(string key, object data, TimeSpan expiration);

        T GetOrAdd<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<T> getData,
            TimeSpan waitForCalculateTimeout = default, bool skipSerialization = false);

        Task<T> GetOrAddAsync<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<Task<T>> getData,
            TimeSpan waitForCalculateTimeout = default, bool skipSerialization = false);

        T GetOrAdd<T>(string cacheKey, TimeSpan expiration, Func<T> getData,
            TimeSpan waitForCalculateTimeout = default);

        Task<T> GetOrAddAsync<T>(string cacheKey, TimeSpan expiration, Func<Task<T>> getData,
            TimeSpan waitForCalculateTimeout = default);
    }
}
