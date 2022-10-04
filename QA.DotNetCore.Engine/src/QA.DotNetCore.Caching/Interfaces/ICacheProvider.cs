using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Interfaces
{
    public delegate IEnumerable<TValue> DataValuesFactoryDelegate<TId, TValue>(IEnumerable<CacheInfo<TId>> missingCacheInfos);
    public delegate Task<IEnumerable<TValue>> AsyncDataValuesFactoryDelegate<TId, TValue>(IEnumerable<CacheInfo<TId>> missingCacheInfos);

    /// <summary>
    /// General-purpose cache provider interface. Supports only serializable objects.
    /// </summary>
    public interface ICacheProvider
    {
        IEnumerable<TResult> Get<TResult>(IEnumerable<string> keys);

        /// <summary>
        /// Проверяет наличие данных в кэше
        /// </summary>
        /// <param name="keys">Ключи</param>
        /// <returns>Список наличия ключей к кэше</returns>
        IEnumerable<bool> IsSet(IEnumerable<string> keys);

        /// <summary>
        /// Пытается получить данные из кэша по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="result">Результат</param>
        /// <returns></returns>
        // TODO: Ensure non null consistency between Add and Get methods (throughout implementations).
        bool TryGetValue<TResult>(string key, [NotNullWhen(true)] out TResult result);

        /// <summary>
        /// Записывает данные в кэш, маркирует эту запись тегами
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="tags">Теги</param>
        /// <param name="expiration">Время кэширования (sliding expiration)</param>
        void Add(object data, string key, string[] tags, TimeSpan expiration);

        // TODO: Move to extensions (to minimize interface).
        T GetOrAdd<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<T> getData, TimeSpan waitForCalculateTimeout = default);

        // TODO: Add async version of method.
        TResult[] GetOrAdd<TId, TResult>(
            CacheInfo<TId>[] cacheInfos,
            DataValuesFactoryDelegate<TId, TResult> dataValuesFactory,
            TimeSpan waitForCalculateTimeout = default);

        Task<T> GetOrAddAsync<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<Task<T>> getData, TimeSpan waitForCalculateTimeout = default);

        /// <summary>
        /// Очищает кэш по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.")]
        void Invalidate(string key);

        /// <summary>
        /// Инвалидирует все записи в кэше по тегам
        /// </summary>
        /// <param name="tags">Теги</param>
        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.")]
        void InvalidateByTags(params string[] tags);
    }
}
