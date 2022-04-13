namespace QA.DotNetCore.Caching.Interfaces
{
    public interface ICacheInvalidator
    {
        /// <summary>
        /// Очищает кэш по ключу
        /// </summary>
        /// <param name="key">Ключ</param>
        void Invalidate(string key);

        /// <summary>
        /// Инвалидирует все записи в кэше по тегу
        /// </summary>
        /// <param name="tag">Тег</param>
        void InvalidateByTag(string tag);

        /// <summary>
        /// Инвалидирует все записи в кэше по тегам
        /// </summary>
        /// <param name="tags">Теги</param>
        void InvalidateByTags(params string[] tags);

    }
}
