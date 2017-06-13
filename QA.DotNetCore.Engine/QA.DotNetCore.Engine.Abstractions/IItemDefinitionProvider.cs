using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions
{
    /// <summary>
    /// Интерфейс провайдера доступных типов элементов структуры сайта
    /// </summary>
    public interface IItemDefinitionProvider
    {
        IEnumerable<IItemDefinition> GetAllDefinitions();

        IItemDefinition GetById(string discriminator);
    }
}
