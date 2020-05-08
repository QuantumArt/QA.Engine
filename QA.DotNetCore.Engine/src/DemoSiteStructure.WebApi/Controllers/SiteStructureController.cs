using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using QA.DotNetCore.Engine.Abstractions;
using QA.DotNetCore.Engine.QpData;

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
        public ActionResult Get(int? id)
        {
            var storage = AbstractItemStorageProvider.Get();
            var item = id.HasValue ? storage.Get(id.Value) : storage.Root;

            return new JsonResult(item, IgnoreParentSerializeSettings());
        }

        private static JsonSerializerSettings IgnoreParentSerializeSettings()
        {
            return  new JsonSerializerSettings
            {
                ContractResolver = new IgnoreParentSerializerContractResolver()
            };
        }
    }

    public class IgnoreParentSerializerContractResolver : DefaultContractResolver
    {
        public IgnoreParentSerializerContractResolver()
        {
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.DeclaringType.IsAssignableFrom(typeof(AbstractItem)) && property.PropertyName == "Parent")
            {
                property.ShouldSerialize =
                    _ =>
                    { 
                        return false;
                    };
            }

            return property;
        }
    }
}
