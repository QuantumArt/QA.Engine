namespace QA.DotNetCore.Engine.QpData.Persistent.Interfaces
{
    public interface IUnitOfWork
    {
        IAbstractItemRepository AbstractItemRepository { get; }
    }
}
