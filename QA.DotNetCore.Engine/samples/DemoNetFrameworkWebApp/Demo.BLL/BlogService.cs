using System;
using Demo.DAL;
using QA.DotNetCore.Caching;

namespace Demo.BLL
{
    public class BlogService
    {
        private readonly ICacheProvider _cacheProvider;

        public BlogService(ICacheProvider cacheProvider)
        {
            _cacheProvider = cacheProvider;
        }

    }
}
