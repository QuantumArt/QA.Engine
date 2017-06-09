using System.Collections.Generic;

namespace QA.DotNetCore.Engine.QpData
{
    public interface IItemDefinitionProvider
    {
        IEnumerable<ItemDefinition> GetAllDefinitions();

        ItemDefinition GetById(string discriminator);
    }
}
