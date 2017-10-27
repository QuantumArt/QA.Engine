using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System.Collections.Generic;
using Quantumart.QPublishing.Database;
using System.Collections;
using Quantumart.QPublishing.Info;
using QA.DotNetCore.OnScreenAdmin.Web.Models;
using System;

namespace QA.DotNetCore.OnScreenAdmin.Web.Controllers
{
    [Route("api")]
    public class ApiController
    {
        IMetaInfoRepository _metaInfoRepository;
        IItemDefinitionRepository _itemDefinitionRepository;
        DBConnector _dbConnector;

        public ApiController(IMetaInfoRepository metaInfoRepository, IItemDefinitionRepository itemDefinitionRepository, DBConnector dbConnector)
        {
            _metaInfoRepository = metaInfoRepository;
            _itemDefinitionRepository = itemDefinitionRepository;
            _dbConnector = dbConnector;
        }

        [HttpGet("meta")]
        public ApiResult Meta(int siteId, string contentNetName)
        {
            try
            {
                return ApiResult.Success(_metaInfoRepository.GetContent(contentNetName, siteId));
            }
            catch (Exception ex)
            {
                return ApiResult.Error(ex.Message);
            }
        }

        [HttpGet("availableWidgets")]
        public ApiResult AvailableWidgets(int siteId)
        {
            try
            {
                return ApiResult.Success<IEnumerable<ItemDefinitionPersistentData>>(_itemDefinitionRepository.GetAllItemDefinitions(siteId, true).Where(d => !d.IsPage).ToList());
            }
            catch (Exception ex)
            {
                return ApiResult.Error(ex.Message);
            }
        }

        [HttpGet("moveWidget")]
        public ApiResult MoveWidget(int widgetId, int newParentId, string zoneName)
        {
            try
            {
                //получим id контента (это должен быть AbstractItem)
                var contentId = _dbConnector.GetContentIdForItem(widgetId);

                //получим названия полей Parent и ZoneName в найденном контенте, чтобы использовать их для метода MassUpdate
                //на разных базах эти названия в теории могут отличаться, инвариант - это NetName
                var parentField = _metaInfoRepository.GetContentAttributeByNetName(contentId, "Parent");
                if (parentField == null)
                    return ApiResult.Error($"Field with netname 'Parent' not found in content {contentId}");
                var zoneNameField = _metaInfoRepository.GetContentAttributeByNetName(contentId, "ZoneName");
                if (zoneNameField == null)
                    return ApiResult.Error($"Field with netname 'ZoneName' not found in content {contentId}");

                var widgetUpdates = new Dictionary<string, string>
                {
                    [SystemColumnNames.Id] = widgetId.ToString(),
                    [parentField.ColumnName] = newParentId.ToString(),
                    [zoneNameField.ColumnName] = zoneName
                };

                _dbConnector.MassUpdate(contentId, new[] { widgetUpdates }, 1);

                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Error(ex.Message);
            }
        }
    }
}
