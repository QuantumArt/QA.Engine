namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    public interface ITargetingProviderSource
    {
        /// <summary>
        /// Источник данных для таргетинга
        /// </summary>
        TargetingSource Source { get; }
    }
}
