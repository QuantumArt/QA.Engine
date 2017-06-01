namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Фабрика по созданию элементов структуры сайта QP
    /// </summary>
    public interface IAbstractItemFactory
    {
        AbstractItem Create(string discriminator);
    }
}
