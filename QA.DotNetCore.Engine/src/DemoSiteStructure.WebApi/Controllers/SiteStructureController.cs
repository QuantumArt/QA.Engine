using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Abstractions;

namespace DemoSiteStructure.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SiteStructureController : ControllerBase
    {
        public SiteStructureController(IAbstractItemStorageProvider abstractItemStorageProvider)
        {
            AbstractItemStorageProvider = abstractItemStorageProvider;
        }

        public IAbstractItemStorageProvider AbstractItemStorageProvider { get; }

        [HttpGet]
        public IAbstractItem Get(int? id)
        {
            var storage = AbstractItemStorageProvider.Get();
            var item = id.HasValue ? storage.Get(id.Value) : storage.Root;
            return item;
        }
    }
}
