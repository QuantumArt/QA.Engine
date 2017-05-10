namespace Common.Persistent
{
    public interface IUnitOfWork
    {
        IAbstractItemRepository AbstractItemRepository { get; }
    }
}
