using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DotNetCore.Engine.Routing.Mappers
{
    /// <summary>
    /// Атрибут, которым должны быть помечены контроллеры, обрабатывающие запросы от страниц структуры сайта при использовании AttributeConventionalControllerMapper.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SiteStructureControllerAttribute : Attribute
    {
        public Type ItemType { get; private set; }

        public SiteStructureControllerAttribute(Type itemType)
        {
            ItemType = itemType;
        }
    }
}
