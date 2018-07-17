using QA.DotNetCore.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace QA.DotNetCore.Engine.Xml
{
    public class XmlAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        private readonly XmlStorageSettings _xmlStorageSettings;
        private readonly XmlAbstractItemFactory _abstractItemFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly Lazy<AbstractItemStorage> _storage;

        public XmlAbstractItemStorageProvider(XmlStorageSettings xmlStorageSettings,
            XmlAbstractItemFactory abstractItemFactory,
            IServiceProvider serviceProvider)
        {
            _xmlStorageSettings = xmlStorageSettings;
            _abstractItemFactory = abstractItemFactory;
            _serviceProvider = serviceProvider;
            _storage = new Lazy<AbstractItemStorage>(() => Build());
        }

        public AbstractItemStorage Get()
        {
            return _storage.Value;
        }

        private AbstractItemStorage Build()
        {
            using (var fs = new FileStream(_xmlStorageSettings.FilePath, FileMode.Open, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs))//use encoding option here
                {
                    using (var xmlReader = XmlReader.Create(sr))
                    {
                        //переменные контекста
                        XmlAbstractItem root = null;
                        var contextStack = new Stack<TreeItemContext>();
                        var lastDepth = 0;
                        var lastId = 0;

                        while (xmlReader.Read())
                        {
                            if (xmlReader.NodeType == XmlNodeType.Element)
                            {
                                //текущая глубина в дереве
                                var depth = xmlReader.Depth;
                                if (depth == 0 && root != null)//по идее несколько корневых элементов не допустит XmlReader, но проверку всё равно сделаю
                                    throw new Exception("Can be only one root element in site structure.");

                                //получим имя и атрибуты элемента
                                var typeName = xmlReader.Name;
                                var attributes = new Dictionary<string, string>();
                                if (xmlReader.AttributeCount > 0)
                                {
                                    for (int attInd = 0; attInd < xmlReader.AttributeCount; attInd++)
                                    {
                                        xmlReader.MoveToAttribute(attInd);
                                        attributes[xmlReader.Name] = xmlReader.Value;
                                    }
                                    xmlReader.MoveToElement();
                                }

                                //получим parent
                                TreeItemContext parentContext = null;
                                if (depth > lastDepth)
                                {
                                    //глубина увеличилась - значит элемент является дочерним к предыдущему
                                    parentContext = contextStack.Peek();
                                }
                                else if (depth > 0) //0 - это root
                                {
                                    //если глубина не увеличилась (а может и уменьшилась) - значит нужно искать родителя на соответствующем уровне стека
                                    for (var i = lastDepth; i >= depth; i--)
                                    {
                                        contextStack.Pop();
                                    }
                                    parentContext = contextStack.Peek();
                                }

                                if (parentContext != null && parentContext.Item == null)
                                {
                                    //если родительский контекст пуст, то даже не пытаемся создать элемент и добавим пустой контекст в стек
                                    //другими словами, игнорируем все дочерние элементы у несуществующей страницы
                                    contextStack.Push(new TreeItemContext());
                                }
                                else
                                {
                                    //создадим элемент
                                    var element = _abstractItemFactory.Create(typeName);
                                    if (element != null)
                                    {
                                        element.Init(attributes, ++lastId, parentContext?.Item);
                                        contextStack.Push(new TreeItemContext { Item = element });
                                    }
                                    else
                                    {
                                        contextStack.Push(new TreeItemContext());//в стек добавим пустой контекст
                                    }
                                    if (parentContext == null)
                                        root = element;
                                }
                                
                                lastDepth = depth;
                            }
                        }

                        if (root == null)
                            throw new Exception("Site structure is empty.");

                        return new AbstractItemStorage(root, _serviceProvider);
                    }
                }
            }
        }
    }

    internal class TreeItemContext
    {
        public XmlAbstractItem Item { get; set; }
    }
}
