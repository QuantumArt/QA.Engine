using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.QpData.Interfaces
{
    /// <summary>
    /// Интерфейс строителя структуры сайта
    /// </summary>
    public interface IAbstractItemStorageBuilder
    {
        AbstractItemStorage Build();
        /// <summary>
        /// Net-имена контентов, участвующих в строительстве структуры сайта
        /// </summary>
        string[] UsedContentNetNames { get; }
    }
}
