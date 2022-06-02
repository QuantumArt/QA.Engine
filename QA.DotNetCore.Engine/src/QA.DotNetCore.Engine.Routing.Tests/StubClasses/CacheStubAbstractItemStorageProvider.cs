using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using System;

namespace QA.DotNetCore.Engine.Routing.Tests.StubClasses
{
    public class CacheStubAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        private readonly IMemoryCacheProvider _memoryCacheProvider;

        public CacheStubAbstractItemStorageProvider(IMemoryCacheProvider memoryCacheProvider)
        {
            _memoryCacheProvider = memoryCacheProvider;
        }

        public AbstractItemStorage Get()
        {
            return _memoryCacheProvider.GetOrAdd(
                "SomeKey",
                TimeSpan.FromSeconds(5),
                new Func<AbstractItemStorage>(() => null));
        }
    }
}
