using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.Routing.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Widgets.Mappers
{
    public class AttributeConventionalComponentMapper : IComponentMapper
    {
        readonly Dictionary<Type, Type> _componentMap = new Dictionary<Type, Type>();

        public AttributeConventionalComponentMapper(ITypeFinder typeFinder)
        {
            var contollerTypes = typeFinder.GetTypesOf<ViewComponent>();
            foreach (var t in contollerTypes.Values)
            {
                var attr = t.GetCustomAttributes(typeof(SiteStructureComponentAttribute), false).Cast<SiteStructureComponentAttribute>().FirstOrDefault();
                if (attr != null && attr.ItemType != null)
                {
                    if (_componentMap.ContainsKey(attr.ItemType))
                        throw new Exception($"There are two or many component mapped on {attr.ItemType.Name}");

                    _componentMap[attr.ItemType] = t;
                }
            }
        }

        public string Map(IAbstractItem widget)
        {
            var requestedType = widget.GetType();

            //будем искать компонент, помеченный типом переданного виджета или, если не найдём, то базовым для переданной страницы типом
            while (requestedType != typeof(object))
            {
                if (_componentMap.ContainsKey(requestedType))
                {
                    var componentName = _componentMap[requestedType].Name;
                    return componentName.EndsWith("Component") ? componentName.Substring(0, componentName.Length - "Component".Length) : componentName;
                }
                requestedType = requestedType.BaseType;
            }
            return null;
        }
    }
}
