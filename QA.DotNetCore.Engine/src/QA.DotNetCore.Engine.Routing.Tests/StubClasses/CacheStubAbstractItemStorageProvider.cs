using QA.DotNetCore.Caching.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using System;

namespace QA.DotNetCore.Engine.Routing.Tests.StubClasses
{
    public class CacheStubAbstractItemStorageProvider : IAbstractItemStorageProvider
    {
        private readonly ICacheProvider _cacheProvider;

        public CacheStubAbstractItemStorageProvider(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

        public AbstractItemStorage Get()
        {
            return _cacheProvider.GetOrAdd("SomeKey", TimeSpan.FromSeconds(5), new Func<AbstractItemStorage>(() => null));
        }
    }
}
