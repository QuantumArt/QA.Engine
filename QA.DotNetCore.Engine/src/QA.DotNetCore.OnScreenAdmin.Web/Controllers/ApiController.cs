using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.OnScreenAdmin.Web.Auth;
using QA.DotNetCore.Engine.QpData.Replacements;
using QA.DotNetCore.OnScreenAdmin.Web.Models;
using QA.DotNetCore.OnScreenAdmin.Web.Models.AbTests;
using Quantumart.QPublishing.Database;
using Quantumart.QPublishing.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using QA.DotNetCore.Caching.Interfaces;

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
        IQpUrlResolver _qpUrlResolver;
        ICacheProvider _cacheProvider;
        IQpContentCacheTagNamingProvider _qpContentCacheTagNamingProvider;

        public ApiController(IMetaInfoRepository metaInfoRepository,
            IItemDefinitionRepository itemDefinitionRepository,
            DBConnector dbConnector,
            IAbTestRepository abTestRepository,
            IQpUrlResolver qpUrlResolver,
            ICacheProvider cacheProvider,
            IQpContentCacheTagNamingProvider qpContentCacheTagNamingProvider)
        {
            _metaInfoRepository = metaInfoRepository;
            _itemDefinitionRepository = itemDefinitionRepository;
            _dbConnector = dbConnector;
            _qpUrlResolver = qpUrlResolver;
            _abTestRepository = abTestRepository;
            _cacheProvider = cacheProvider;
            _qpContentCacheTagNamingProvider = qpContentCacheTagNamingProvider;
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
                return ApiResult.Error(Response, ex.Message);
            }
        }

        [HttpGet("availableWidgets")]
        public ApiResult AvailableWidgets(int siteId, bool isStage = true)
        {
            try
            {
                var content = _metaInfoRepository.GetContent("QPDiscriminator", siteId);
                if (content == null)
                    return ApiResult.Error(Response, $"Not found QPDiscriminator content in site {siteId}");

                var cacheTag = new string[1] { _qpContentCacheTagNamingProvider.Get(content.ContentName, siteId, isStage) };

                var widgetDefinitions = _cacheProvider.GetOrAdd($"AvailableWidgets_{siteId}_{isStage}", cacheTag, TimeSpan.FromSeconds(30), () => {
                    var result = _itemDefinitionRepository
                        .GetAllItemDefinitions(siteId, isStage)
                        .Where(d => !d.IsPage)
                        .ToList();

                    var baseIconUrl = _qpUrlResolver.UrlForImage(siteId, content.ContentId, "IconUrl");

                    foreach (var w in result)
                    {
                        w.IconUrl = (baseIconUrl ?? "") + "/" + w.IconUrl;
                    }

                    return result;
                });

                return ApiResult.Success<IEnumerable<ItemDefinitionPersistentData>>(widgetDefinitions);
            }
            catch (Exception ex)
            {
                return ApiResult.Error(Response, ex.Message);
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
                    return ApiResult.Error(Response, $"Not found content with article {widgetId}");

                //получим названия полей Parent и ZoneName в найденном контенте, чтобы использовать их для метода MassUpdate
                //на разных базах эти названия в теории могут отличаться, инвариант - это NetName
                var parentField = _metaInfoRepository.GetContentAttributeByNetName(contentId, "Parent");
                if (parentField == null)
                    return ApiResult.Error(Response, $"Field with netname 'Parent' not found in content {contentId}");
                var zoneNameField = _metaInfoRepository.GetContentAttributeByNetName(contentId, "ZoneName");
                if (zoneNameField == null)
                    return ApiResult.Error(Response, $"Field with netname 'ZoneName' not found in content {contentId}");

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
                return ApiResult.Error(Response, ex.Message);
            }
        }

        [HttpGet("abtests/info")]
        public ApiResult ContainersByAbTests(int siteId, bool isStage, int[] cids)
        {
            try
            {
                var cacheTag = new string[1] { _qpContentCacheTagNamingProvider.GetByNetName(_abTestRepository.AbTestNetName, siteId, isStage) };
                var tests = _cacheProvider.GetOrAdd($"AllTests_{siteId}_{isStage}", cacheTag, TimeSpan.FromSeconds(30), () =>
                {
                    return _abTestRepository.GetAllTests(siteId, isStage); ;
                });
                
                var containersCacheTags = new string[4] {
                    _abTestRepository.AbTestNetName,
                    _abTestRepository.AbTestContainerNetName,
                    _abTestRepository.AbTestScriptNetName,
                    _abTestRepository.AbTestRedirectNetName
                }.Select(c => _qpContentCacheTagNamingProvider.GetByNetName(c, siteId, isStage))
                .Where(t => t != null)
                .ToArray();
                var containers = _cacheProvider.GetOrAdd($"AllTestContainers_{siteId}_{isStage}", containersCacheTags, TimeSpan.FromSeconds(30), () =>
                {
                    return _abTestRepository.GetAllTestsContainers(siteId, isStage);
                });

                var result = containers
                    .Where(c => cids.Contains(c.Id))
                    .GroupBy(c => c.TestId)
                    .Select(g => new AbTestInfo(tests.FirstOrDefault(t => t.Id == g.Key), g))
                    .Where(t => t.Id > 0)
                    .ToList();

                return ApiResult.Success<IEnumerable<AbTestInfo>>(result);
            }
            catch (Exception ex)
            {
                return ApiResult.Error(Response, ex.Message);
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
                    return ApiResult.Error(Response, $"Not found content with article {testId}");

                //получим название поля Enabled в найденном контенте, чтобы использовать для метода MassUpdate
                //на разных базах эти названия в теории могут отличаться, инвариант - это NetName
                var enabledField = _metaInfoRepository.GetContentAttributeByNetName(contentId, "Enabled");
                if (enabledField == null)
                    return ApiResult.Error(Response, $"Field with netname 'Enabled' not found in content {contentId}");

                var testUpdate = new Dictionary<string, string>
                {
                    [SystemColumnNames.Id] = testId.ToString(),
                    [enabledField.ColumnName] = (value ? 1 : 0).ToString()
                };

                _dbConnector.MassUpdate(contentId, new[] { testUpdate }, GetUserId());

                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Error(Response, ex.Message);
            }
        }

        [HttpPost("abtests/create")]
        public ApiResult CreateAbTest([FromBody] AbTestCreateModel model)
        {
            try
            {
                //создание теста - это создание записей сразу в нескольких контентах
                //проверим наличие всех нужных нам контентов в QP
                var abTestContent = _metaInfoRepository.GetContent("AbTest", model.SiteId);
                if (abTestContent == null)
                    return ApiResult.Error(Response, $"Not found AbTest content in site {model.SiteId}");

                var containerContent = _metaInfoRepository.GetContent("AbTestBaseContainer", model.SiteId);
                if (containerContent == null)
                    return ApiResult.Error(Response, $"Not found AbTestBaseContainer content in site {model.SiteId}");

                var scriptContainerContent = _metaInfoRepository.GetContent("AbTestScriptContainer", model.SiteId);
                if (scriptContainerContent == null)
                    return ApiResult.Error(Response, $"Not found AbTestScriptContainer content in site {model.SiteId}");

                var scriptContent = _metaInfoRepository.GetContent("AbTestScript", model.SiteId);
                if (scriptContent == null)
                    return ApiResult.Error(Response, $"Not found AbTestScript content in site {model.SiteId}");

                var redirectContainerContent = _metaInfoRepository.GetContent("AbTestClientRedirectContainer", model.SiteId);
                if (redirectContainerContent == null)
                    return ApiResult.Error(Response, $"Not found AbTestClientRedirectContainer content in site {model.SiteId}");

                var redirectContent = _metaInfoRepository.GetContent("AbTestClientRedirect", model.SiteId);
                if (redirectContent == null)
                    return ApiResult.Error(Response, $"Not found AbTestClientRedirect content in site {model.SiteId}");

                //хотим создать все записи сразу опубликованными (цель же ускорить работу контент-редактора)
                var publishedStatus = _dbConnector.GetStatusTypeId(model.SiteId, "Published");

                var testCreateFields = PrepareMassUpdateDictionaryForCreate(abTestContent, publishedStatus, new Dictionary<string, string>
                {
                    ["Title"] = model.Title,
                    ["Comment"] = model.Comment,
                    ["Enabled"] = "0",//только что созданный тест всегда делаем выключенным
                    ["Percentage"] = GetPercentageAsString(model.Percentage),
                    ["StartDate"] = model.StartDate?.ToString(),
                    ["EndDate"] = model.EndDate?.ToString(),
                });
                
                _dbConnector.MassUpdate(abTestContent.ContentId, new[] { testCreateFields }, GetUserId());

                //id только что созданного теста
                var testId = testCreateFields[SystemColumnNames.Id];

                foreach (var container in model.Containers)
                {
                    int containerTypeId;
                    switch (container.Type)
                    {
                        case AbTestContainerType.Script:
                            containerTypeId = scriptContainerContent.ContentId;
                            break;
                        case AbTestContainerType.ClientRedirect:
                            containerTypeId = redirectContainerContent.ContentId;
                            break;
                        default:
                            continue;
                    }
                    var containerCreateFields = PrepareMassUpdateDictionaryForCreate(containerContent, publishedStatus, new Dictionary<string, string>
                    {
                        ["ParentTest"] = testId,
                        ["Description"] = container.Description,
                        ["AllowedUrlPatterns"] = container.AllowedUrlPatterns,
                        ["DeniedUrlPatterns"] = container.DeniedUrlPatterns,
                        ["Domain"] = container.Domain,
                        ["Precondition"] = container.Precondition,
                        ["Arguments"] = container.Arguments,
                        ["Type"] = containerTypeId.ToString()
                    });

                    _dbConnector.MassUpdate(containerContent.ContentId, new[] { containerCreateFields }, GetUserId());

                    var baseContainerId = containerCreateFields[SystemColumnNames.Id];

                    if (container.Type == AbTestContainerType.Script)
                    {
                        var scriptContainerCreateFields = PrepareMassUpdateDictionaryForCreate(scriptContainerContent, publishedStatus, new Dictionary<string, string>
                        {
                            ["BaseContainer"] = baseContainerId
                        });

                        _dbConnector.MassUpdate(scriptContainerContent.ContentId, new[] { scriptContainerCreateFields }, GetUserId());

                        var containerId = scriptContainerCreateFields[SystemColumnNames.Id];

                        var scriptVariants = new List<Dictionary<string, string>>();
                        var versionNumber = 0;
                        foreach (var variant in container.Variants)
                        {
                            if (!String.IsNullOrWhiteSpace(variant.Value))
                            { 
                                scriptVariants.Add(PrepareMassUpdateDictionaryForCreate(scriptContent, publishedStatus, new Dictionary<string, string>
                                {
                                    ["Container"] = containerId,
                                    ["VersionNumber"] = versionNumber.ToString(),
                                    ["ScriptText"] = variant.Value,
                                    ["Description"] = String.IsNullOrWhiteSpace(variant.Description) ? "-" : variant.Description
                                }));
                            }
                            versionNumber++;
                        }

                        _dbConnector.MassUpdate(scriptContent.ContentId, scriptVariants, GetUserId());
                    }
                    else if (container.Type == AbTestContainerType.ClientRedirect)
                    {
                        var redirectContainerCreateFields = PrepareMassUpdateDictionaryForCreate(redirectContainerContent, publishedStatus, new Dictionary<string, string>
                        {
                            ["BaseContainer"] = baseContainerId
                        });

                        _dbConnector.MassUpdate(redirectContainerContent.ContentId, new[] { redirectContainerCreateFields }, GetUserId());

                        var containerId = redirectContainerCreateFields[SystemColumnNames.Id];

                        var redirectVariants = new List<Dictionary<string, string>>();
                        var versionNumber = 0;
                        foreach (var variant in container.Variants)
                        {
                            if (!String.IsNullOrWhiteSpace(variant.Value))
                            {
                                redirectVariants.Add(PrepareMassUpdateDictionaryForCreate(redirectContent, publishedStatus, new Dictionary<string, string>
                                {
                                    ["Container"] = containerId,
                                    ["VersionNumber"] = versionNumber.ToString(),
                                    ["RedirectUrl"] = variant.Value,
                                }));
                            }
                            versionNumber++;
                        }
                        
                        _dbConnector.MassUpdate(redirectContent.ContentId, redirectVariants, GetUserId());
                    }
                }

                return ApiResult.Success();
            }
            catch (Exception ex)
            {
                return ApiResult.Error(Response, ex.Message);
            }
        }

        private Dictionary<string, string> PrepareMassUpdateDictionaryForCreate(ContentPersistentData content, int status, Dictionary<string, string> fields)
        {
            var result = new Dictionary<string, string>
            {
                [SystemColumnNames.Id] = "0",
                [SystemColumnNames.StatusTypeId] = status.ToString(),
            };
            foreach (var field in fields)
            {
                var attr = content.ContentAttributes.FirstOrDefault(a => a.NetName == field.Key);
                if (attr == null)
                    throw new Exception($"Field with net name '{field.Key}' not found in content '{content.ContentName}'");
                result[attr.ColumnName] = field.Value;
            }
            return result;
        }

        private string GetPercentageAsString(decimal[] percentage)
        {
            return String.Join(";", percentage.Select(p => (int)Math.Round(p)));
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
