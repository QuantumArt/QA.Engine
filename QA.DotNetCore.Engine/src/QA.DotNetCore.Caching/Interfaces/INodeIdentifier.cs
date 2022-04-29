namespace QA.DotNetCore.Caching.Interfaces
{
    /// <summary>
    /// Provides information about current application instance.
    /// </summary>
    public interface INodeIdentifier
    {
        /// <summary>
        /// Obtain unique identifier of application instance.
        /// </summary>
        /// <returns>Unique identifier.</returns>
        string GetUniqueId();
    }
}
