namespace QA.DotNetCore.Engine.Abstractions.Targeting
{
    /// <summary>
    /// Назначедие, для какой цели будут применяться фильтры
    /// </summary>
    public enum TargetingDestination
    {
        /// <summary>
        ///  Для применения к древовидной структуре (Parent-Children)
        /// </summary>
        Structure = 0,
        /// <summary>
        /// Для применения к плоской коллекции нод
        /// </summary>
        Nodes = 1
    }
}
