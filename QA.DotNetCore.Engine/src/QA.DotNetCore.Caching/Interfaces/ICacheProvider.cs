using QA.DotNetCore.Caching.Exceptions;
using System;
using System.Threading.Tasks;

namespace QA.DotNetCore.Caching.Interfaces
{
    public interface ICacheProvider
    {
        /// <summary>
        /// Получает данные из кэша по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// Записывает данные в кэш
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="cacheTimeInSeconds">Время кэширования в секундах</param>
        void Set(string key, object data, int cacheTimeInSeconds);

        /// <summary>
        /// Записывает данные в кэш
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="expiration">Время кэширования (sliding expiration)</param>
        void Set(string key, object data, TimeSpan expiration);

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
        /// Очищает кэш по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.")]
        void Invalidate(string key);

        /// <summary>
        /// Записывает данные в кэш, маркирует эту запись тегами
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="data">Данные</param>
        /// <param name="tags">Теги</param>
        /// <param name="expiration">Время кэширования (sliding expiration)</param>
        void Add(object data, string key, string[] tags, TimeSpan expiration);

        /// <summary>
        /// Инвалидирует все записи в кэше по тегу
        /// </summary>
        /// <param name="tag">Тег</param>
        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.")]
        void InvalidateByTag(string tag);

        /// <summary>
        /// Инвалидирует все записи в кэше по тегам
        /// </summary>
        /// <param name="tags">Теги</param>
        [Obsolete("Use " + nameof(ICacheInvalidator) + " interface instead.")]
        void InvalidateByTags(params string[] tags);

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
        T GetOrAdd<T>(string cacheKey, TimeSpan expiration, Func<T> getData,
            TimeSpan waitForCalculateTimeout = default(TimeSpan));

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
        T GetOrAdd<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<T> getData,
            TimeSpan waitForCalculateTimeout = default(TimeSpan));

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
        Task<T> GetOrAddAsync<T>(string cacheKey, TimeSpan expiration, Func<Task<T>> getData,
            TimeSpan waitForCalculateTimeout = default(TimeSpan));

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
        Task<T> GetOrAddAsync<T>(string cacheKey, string[] tags, TimeSpan expiration, Func<Task<T>> getData,
            TimeSpan waitForCalculateTimeout = default(TimeSpan));
    }
}
