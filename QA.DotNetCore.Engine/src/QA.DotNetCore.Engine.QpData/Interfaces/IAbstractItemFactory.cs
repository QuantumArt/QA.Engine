namespace QA.DotNetCore.Engine.QpData.Interfaces
{
    /// <summary>
    /// Фабрика по созданию элементов структуры сайта QP
    /// </summary>
    public interface IAbstractItemFactory
    {
        AbstractItem Create(string discriminator);
    }
}
