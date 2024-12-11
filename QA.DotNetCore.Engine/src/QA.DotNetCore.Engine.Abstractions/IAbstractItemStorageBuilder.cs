using System;

namespace QA.DotNetCore.Engine.Abstractions
{
    /// <summary>
    /// Интерфейс строителя структуры сайта
    /// </summary>
    public interface IAbstractItemStorageBuilder
    {
        AbstractItemStorage Build();

        void SetServiceProvider(IServiceProvider serviceProvider);
    }
}
