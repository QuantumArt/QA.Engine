using QA.DotNetCore.Engine.QpData.Interfaces;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace QA.DotNetCore.Engine.QpData
{
    public class AbstractItemFactory : IAbstractItemFactory
    {
        readonly IItemDefinitionProvider _itemDefinitionProvider;
        readonly IServiceProvider _serviceProvider;
        public AbstractItemFactory(IItemDefinitionProvider itemDefinitionProvider, IServiceProvider serviceProvider)
        {
            _itemDefinitionProvider = itemDefinitionProvider;
            _serviceProvider = serviceProvider;
        }

        public AbstractItem Create(string discriminator)
        {
            var definition = _itemDefinitionProvider.GetById(discriminator);
            if (definition != null && definition.Type != null)
            {
                return ActivatorUtilities.CreateInstance(_serviceProvider, definition.Type) as AbstractItem;
                //return Activator.CreateInstance(definition.Type) as AbstractItem;
            }

            return null;
        }
    }
}
