using System.Collections.Generic;

namespace QA.DotNetCore.Engine.Abstractions
{
    public interface IItemDefinitionProvider
    {
        IEnumerable<IItemDefinition> GetAllDefinitions();

        IItemDefinition GetById(string discriminator);
    }
}
