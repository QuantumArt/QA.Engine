using System;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Interfaces
{
    /// <summary>
    /// General-purpose cache provider interface. Supports only serializable objects.
    /// </summary>
    public interface ICacheProvider
    {
        /// <summary>
        /// Получает данные из кэша по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// Проверяет наличие данных в кэше
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        bool IsSet(string key);

        /// <summary>
        /// Пытается получить данные из кэша по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="result">Результат</param>
        /// <returns></returns>
        bool TryGetValue(string key, out object result);

        /// <summary>
        /// Записывает данные в кэш, маркирует эту запись тегами
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="tags">Теги</param>
        /// <param name="expiration">Время кэширования (sliding expiration)</param>
        void Add(object data, string key, string[] tags, TimeSpan expiration);

        T GetOrAdd<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<T> getData, TimeSpan waitForCalculateTimeout = default);

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
