using QA.DotNetCore.Engine.Abstractions;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Интерфейс строителя структуры сайта
    /// </summary>
    public interface IAbstractItemStorageBuilder
    {
        AbstractItemStorage Build();
    }
}
