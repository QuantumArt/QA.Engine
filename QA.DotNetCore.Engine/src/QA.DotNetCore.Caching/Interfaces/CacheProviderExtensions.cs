using System;
using System.Linq;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Interfaces
{
    public static class CacheProviderExtensions
    {


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

            cacheProvider.Add(data, key, Array.Empty<string>(), TimeSpan.FromSeconds(cacheTimeInSeconds), false);
        }

        /// <summary>
        /// Записывает данные в кеш.
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="cacheTimeInSeconds">Время кеширования.</param>
        public static void Set(this ICacheProvider cacheProvider, string key, object data, TimeSpan expiration)
        {
            if (cacheProvider is null)
            {
                throw new ArgumentNullException(nameof(cacheProvider));
            }

            cacheProvider.Add(data, key, Array.Empty<string>(), expiration, false);
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
  
    }
}
