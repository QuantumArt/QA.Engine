using QA.DotNetCore.Engine.Abstractions;
using System;

namespace QA.DotNetCore.Engine.Xml
{
    public class XmlAbstractItemFactory
    {
        private readonly ITypeFinder _typeFinder;

        public XmlAbstractItemFactory(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
        }

        public XmlAbstractItem Create(string typeName)
        {
            var typesDictionary = _typeFinder.GetTypesOf<XmlAbstractItem>();
            if (typesDictionary.ContainsKey(typeName))
            {
                return Activator.CreateInstance(typesDictionary[typeName]) as XmlAbstractItem;
            }

            return null;
        }
    }
}
