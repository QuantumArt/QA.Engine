using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;

namespace QA.DotNetCore.OnScreenAdmin.Web.Controllers
{
    [Route("api")]
    public class ApiController
    {
        IMetaInfoRepository _metaInfoRepository;

        public ApiController(IMetaInfoRepository metaInfoRepository)
        {
            _metaInfoRepository = metaInfoRepository;
        }

        [HttpGet("meta")]
        //[ProducesResponseType(typeof(AddressScope), 200)]
        //[ProducesResponseType(typeof(IValidationErrors), 400)]
        public async Task<ContentPersistentData> Meta(int siteId, string contentNetName)
        {
            return _metaInfoRepository.GetContent(contentNetName, siteId);
        }
    }
}
