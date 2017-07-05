using QA.DotNetCore.Engine.QpData.Interfaces;
using System;

namespace QA.DotNetCore.Engine.QpData
{
    public class AbstractItemFactory : IAbstractItemFactory
    {
        readonly IItemDefinitionProvider _itemDefinitionProvider;
        public AbstractItemFactory(IItemDefinitionProvider itemDefinitionProvider)
        {
            _itemDefinitionProvider = itemDefinitionProvider;
        }

        public AbstractItem Create(string discriminator)
        {
            var definition = _itemDefinitionProvider.GetById(discriminator);
            if (definition != null && definition.Type != null)
            {
                return Activator.CreateInstance(definition.Type) as AbstractItem;
            }

            return null;
        }
    }
}
