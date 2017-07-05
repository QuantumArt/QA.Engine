using System;

namespace QA.DotNetCore.Engine.Routing.Mappers
{
    /// <summary>
    /// Атрибут, которым должны быть помечены компоненты, обрабатывающие запросы от страниц структуры сайта при использовании AttributeConventionalComponentMapper.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SiteStructureComponentAttribute : Attribute
    {
        public Type ItemType { get; private set; }

        public SiteStructureComponentAttribute(Type itemType)
        {
            ItemType = itemType;
        }
    }
}
