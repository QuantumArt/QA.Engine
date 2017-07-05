using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.Engine.Routing.Mappers
{
    /// <summary>
    /// Реализация IControllerMapper, основанная на соответствии имени типа страницы и атрибута SiteStructureController контроллера
    /// </summary>
    public class AttributeConventionalControllerMapper : IControllerMapper
    {
        readonly Dictionary<Type, Type> _contollerMap = new Dictionary<Type, Type>();

        public AttributeConventionalControllerMapper(ITypeFinder typeFinder)
        {
            var contollerTypes = typeFinder.GetTypesOf<Controller>();
            foreach (var t in contollerTypes.Values)
            {
                var attr = t.GetCustomAttributes(typeof(SiteStructureControllerAttribute), false).Cast<SiteStructureControllerAttribute>().FirstOrDefault();
                if (attr != null && attr.ItemType != null)
                {
                    if (_contollerMap.ContainsKey(attr.ItemType))
                        throw new Exception($"There are two or many contollers mapped on {attr.ItemType.Name}");

                    _contollerMap[attr.ItemType] = t;
                }
            }
        }

        public string Map(IAbstractItem page)
        {
            var requestedType = page.GetType();

            //будем искать контроллер, помеченный типом переданной страницы или, если не найдём, то базовым для переданной страницы типом
            while (requestedType != typeof(object))
            {
                if (_contollerMap.ContainsKey(requestedType))
                {
                    var contollerName = _contollerMap[requestedType].Name;
                    return contollerName.EndsWith("Controller") ? contollerName.Substring(0, contollerName.Length - "Controller".Length) : contollerName;
                }
                requestedType = requestedType.BaseType;
            }
            return null;
        }
    }
}
