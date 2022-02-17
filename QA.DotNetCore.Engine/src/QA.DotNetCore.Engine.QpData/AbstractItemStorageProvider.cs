using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData.Interfaces;

namespace QA.DotNetCore.Engine.QpData
{
    /// <summary>
    /// Предоставляет доступ к структуре сайта, изготовленной строителем по требованию.
    /// </summary>
    public class AbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        private readonly IAbstractItemStorageBuilder _builder;

        public AbstractItemStorageProvider(IAbstractItemStorageBuilder builder)
        {
            _builder = builder;
        }

        public AbstractItemStorage Get()
        {
            return _builder.Build();
        }
    }
}
