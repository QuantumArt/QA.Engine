using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;

namespace QA.DotNetCore.OnScreenAdmin.Web.Controllers
{
    [Route("api")]
    public class ApiController
    {
        IMetaInfoRepository _metaInfoRepository;
        IItemDefinitionRepository _itemDefinitionRepository;

        public ApiController(IMetaInfoRepository metaInfoRepository, IItemDefinitionRepository itemDefinitionRepository)
        {
            _metaInfoRepository = metaInfoRepository;
            _itemDefinitionRepository = itemDefinitionRepository;
        }

        [HttpGet("meta")]
        //[ProducesResponseType(typeof(AddressScope), 200)]
        //[ProducesResponseType(typeof(IValidationErrors), 400)]
        public ContentPersistentData Meta(int siteId, string contentNetName)
        {
            return _metaInfoRepository.GetContent(contentNetName, siteId);
        }

        [HttpGet("availableWidgets")]
        public IEnumerable<ItemDefinitionPersistentData> AvailableWidgets(int siteId)
        {
            return _itemDefinitionRepository.GetAllItemDefinitions(siteId, true).Where(d => !d.IsPage).ToList();
        }
    }
}
