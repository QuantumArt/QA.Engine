using System;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Interfaces
{
    public static class CacheProviderExtensions
    {
        /// <summary>
        /// Проверяет наличие данных в кэше
        /// </summary>
        /// <param name="keys">Ключ</param>
        /// <returns>Присутствует ли ключ к кэше</returns>
        public static bool IsSet(this ICacheProvider cacheProvider, string key)
        {
            if (cacheProvider is null)
            {
                throw new ArgumentNullException(nameof(cacheProvider));
            }

            return cacheProvider.IsSet(new[] { key }).Single();
        }

        /// <summary>
        /// Получает данные из кэша по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        public static object Get(this ICacheProvider cacheProvider, string key)
        {
            if (cacheProvider is null)
            {
                throw new ArgumentNullException(nameof(cacheProvider));
            }

            return cacheProvider.Get(new[] { key }).Single();
        }

        /// <summary>
        /// Записывает данные в кеш
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="cacheTimeInSeconds">Время кеширования в секундах</param>
        public static void Set(this ICacheProvider cacheProvider, string key, object data, int cacheTimeInSeconds)
        {
            if (cacheProvider is null)
            {
                throw new ArgumentNullException(nameof(cacheProvider));
            }

            cacheProvider.Add(data, key, Array.Empty<string>(), TimeSpan.FromSeconds(cacheTimeInSeconds));
        }

        /// <summary>
        /// Записывает данные в кеш.
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="cacheTimeInSeconds">Время кеширования в секундах</param>
        public static void Set(this ICacheProvider cacheProvider, string key, object data, TimeSpan expiration)
        {
            if (cacheProvider is null)
            {
                throw new ArgumentNullException(nameof(cacheProvider));
            }

            cacheProvider.Add(data, key, Array.Empty<string>(), expiration);
        }

        /// <summary>
        /// Потокобезопасно берет объект из кэша, если его там нет, то вызывает функцию для получения данных
        /// и кладет результат в кэш.
        /// </summary>
        /// <typeparam name="T">тип объектов в кэше</typeparam>
        /// <param name="cacheKey">Ключ</param>
        /// <param name="expiration">время жизни в кэше</param>
        /// <param name="getData">функция для получения данных, если объектов кэше нет. нужно использовать анонимный делегат</param>
        /// <param name="waitForCalculateTimeout">таймаут ожидания параллельными потоками события окончания
        /// вычисления <paramref name="getData"/> по истечении  которого им будет возвращён null.</param>
        public static T GetOrAdd<T>(
            this ICacheProvider cacheProvider,
            string cacheKey,
            TimeSpan expiration,
            Func<T> getData,
            TimeSpan waitForCalculateTimeout = default)
        {
            if (cacheProvider is null)
            {
                throw new ArgumentNullException(nameof(cacheProvider));
            }

            return cacheProvider.GetOrAdd(cacheKey, Array.Empty<string>(), expiration, getData, waitForCalculateTimeout);
        }

        /// <summary>
        /// Потокобезопасно берет объект из кэша, если его там нет, то вызывает функцию для получения данных
        /// и кладет результат в кэш.
        /// </summary>
        /// <typeparam name="T">тип объектов в кэше</typeparam>
        /// <param name="cacheKey">Ключ</param>
        /// <param name="expiration">время жизни в кэше</param>
        /// <param name="getData">функция для получения данных, если объектов кэше нет. нужно использовать анонимный делегат</param>
        /// <param name="waitForCalculateTimeout">таймаут ожидания параллельными потоками события окончания
        /// вычисления <paramref name="getData"/> по истечении  которого им будет возвращён null.</param>
        public static Task<T> GetOrAddAsync<T>(
            this ICacheProvider cacheProvider,
            string cacheKey,
            TimeSpan expiration,
            Func<Task<T>> getData,
            TimeSpan waitForCalculateTimeout = default)
        {
            if (cacheProvider is null)
            {
                throw new ArgumentNullException(nameof(cacheProvider));
            }

            return cacheProvider.GetOrAddAsync(cacheKey, Array.Empty<string>(), expiration, getData, waitForCalculateTimeout);
        }

        /// <summary>
        /// Инвалидирует все записи в кэше по тегу
        /// </summary>
        /// <param name="tag">Тег</param>
        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.")]
        public static void InvalidateByTag(this ICacheProvider cacheProvider, string tag)
        {
            if (cacheProvider is null)
            {
                throw new ArgumentNullException(nameof(cacheProvider));
            }

            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException("tag");
            }

            cacheProvider.InvalidateByTags(new[] { tag });
        }

        /// <summary>
        /// Инвалидирует все записи в кэше по тегу
        /// </summary>
        /// <param name="tag">Тег</param>
        public static void InvalidateByTag(this ICacheInvalidator cacheInvalidator, string tag)
        {
            if (cacheInvalidator is null)
            {
                throw new ArgumentNullException(nameof(cacheInvalidator));
            }

            if (string.IsNullOrEmpty(tag))
            {
                throw new ArgumentNullException("tag");
            }

            cacheInvalidator.InvalidateByTags(new[] { tag });
        }
    }
}
