namespace QA.DotNetCore.Engine.Abstractions
{
    /// <summary>
    /// Интерйфейс класса, предоставляющего доступ к структуре сайта
    /// </summary>
    public interface IAbstractItemStorageProvider
    {
        AbstractItemStorage Get();
    }
}
