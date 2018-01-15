using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.OnScreenAdmin.Web.Auth;
using QA.DotNetCore.OnScreenAdmin.Web.Models;
using QA.DotNetCore.OnScreenAdmin.Web.Models.AbTests;
using Quantumart.QPublishing.Database;
using Quantumart.QPublishing.Info;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.OnScreenAdmin.Web.Controllers
{
    [Route("api")]
    [Authorize]
    public class ApiController : Controller
    {
        IMetaInfoRepository _metaInfoRepository;
        IItemDefinitionRepository _itemDefinitionRepository;
        IAbTestRepository _abTestRepository;
        DBConnector _dbConnector;

        public ApiController(IMetaInfoRepository metaInfoRepository, IItemDefinitionRepository itemDefinitionRepository, IAbTestRepository abTestRepository, DBConnector dbConnector)
        {
            _metaInfoRepository = metaInfoRepository;
            _itemDefinitionRepository = itemDefinitionRepository;
            _dbConnector = dbConnector;
            _abTestRepository = abTestRepository;
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

        [HttpPost("moveWidget")]
        public ApiResult MoveWidget(int widgetId, int newParentId, string zoneName)
        {
            try
            {
                //получим id контента (это должен быть AbstractItem)
                var contentId = _dbConnector.GetContentIdForItem(widgetId);

                if (contentId == 0)
                    return ApiResult.Error($"Not found content with article {widgetId}");

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

                _dbConnector.MassUpdate(contentId, new[] { widgetUpdates }, GetUserId());

                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Error(ex.Message);
            }
        }

        [HttpGet("abtests/info")]
        public ApiResult ContainersByAbTests(int siteId, bool isStage, int[] cids)
        {
            try
            {
                var tests = _abTestRepository.GetActiveTests(siteId, isStage);
                var result = _abTestRepository.GetActiveTestsContainers(siteId, isStage)
                    .Where(c => cids.Contains(c.Id))
                    .GroupBy(c => c.TestId)
                    .Select(g => new AbTestInfo(tests.FirstOrDefault(t => t.Id == g.Key), g))
                    .Where(t => t.Id > 0)
                    .ToList();

                return ApiResult.Success<IEnumerable<AbTestInfo>>(result);
            }
            catch (Exception ex)
            {
                return ApiResult.Error(ex.Message);
            }
        }

        [HttpPost("abtests/switch")]
        public ApiResult SwitchAbTest(int testId, bool value)
        {
            try
            {
                //получим id контента (это должен быть AbTest)
                var contentId = _dbConnector.GetContentIdForItem(testId);

                if (contentId == 0)
                    return ApiResult.Error($"Not found content with article {testId}");

                //получим название поля Enabled в найденном контенте, чтобы использовать для метода MassUpdate
                //на разных базах эти названия в теории могут отличаться, инвариант - это NetName
                var enabledField = _metaInfoRepository.GetContentAttributeByNetName(contentId, "Enabled");
                if (enabledField == null)
                    return ApiResult.Error($"Field with netname 'Enabled' not found in content {contentId}");

                var widgetUpdates = new Dictionary<string, string>
                {
                    [SystemColumnNames.Id] = testId.ToString(),
                    [enabledField.ColumnName] = (value ? 1 : 0).ToString()
                };

                _dbConnector.MassUpdate(contentId, new[] { widgetUpdates }, GetUserId());

                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Error(ex.Message);
            }
        }

        private int GetUserId()
        {
            var identity = User.Identity as QpIdentity;
            if (identity == null)
                throw new InvalidOperationException("QpIdentity not found.");

            return identity.UserId;
        }
    }
}
