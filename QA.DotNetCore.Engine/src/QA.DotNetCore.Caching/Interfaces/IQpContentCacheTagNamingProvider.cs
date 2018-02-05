namespace QA.DotNetCore.Caching.Interfaces
{
    /// <summary>
    /// Интерфейс, задающий правила именования кештегов для контентов qp
    /// </summary>
    public interface IQpContentCacheTagNamingProvider
    {
        string Get(string contentName, int contentId, bool isStage);
        string GetByNetName(string contentNetName, int siteId, bool isStage);
    }
}
