namespace QA.DotNetCore.Engine.Abstractions
{
    /// <summary>
    /// Класс, предоставляющий доступ к структуре сайта
    /// </summary>
    public interface IAbstractItemStorageProvider
    {
        AbstractItemStorage Get();
    }
}
