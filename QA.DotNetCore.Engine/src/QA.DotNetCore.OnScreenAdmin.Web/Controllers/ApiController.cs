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
                var baseIconUrl = _qpUrlResolver.UrlForImage(siteId, content.ContentId, "IconUrl");

                var cacheTag = new string[1] { _qpContentCacheTagNamingProvider.Get(content.ContentName, siteId, isStage) };

                var widgetDefinitions = _cacheProvider.GetOrAdd($"AvailableWidgets_{siteId}_{isStage}", cacheTag, TimeSpan.FromSeconds(30), () => {
                    return _itemDefinitionRepository
                        .GetAllItemDefinitions(siteId, isStage)
                        .Where(d => !d.IsPage);
                });

                foreach (var w in widgetDefinitions)
                {
                    w.IconUrl = (baseIconUrl ?? "") + "/" + w.IconUrl;
                }

                return ApiResult.Success<IEnumerable<ItemDefinitionPersistentData>>(widgetDefinitions.ToList());
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
        public ApiResult CreateAbTest(AbTestCreateModel model)
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

                var testCreateFields = new Dictionary<string, string>
                {
                    [abTestContent.ContentAttributes.First(a => a.NetName == "Title").ColumnName] = model.Title,
                    [abTestContent.ContentAttributes.First(a => a.NetName == "Comment").ColumnName] = model.Comment,
                    [abTestContent.ContentAttributes.First(a => a.NetName == "Enabled").ColumnName] = "0",//только что созданный тест всегда делаем выключенным
                    [abTestContent.ContentAttributes.First(a => a.NetName == "Percentage").ColumnName] = GetPercentageAsString(model.Percentage),
                    [abTestContent.ContentAttributes.First(a => a.NetName == "StartDate").ColumnName] = model.StartDate.ToString(),
                    [abTestContent.ContentAttributes.First(a => a.NetName == "EndDate").ColumnName] = model.EndDate.ToString(),
                };
                
                _dbConnector.MassUpdate(abTestContent.ContentId, new[] { testCreateFields }, GetUserId());

                //id только что созданного теста
                var testId = testCreateFields[SystemColumnNames.Id];

                var containersToCreate = model.Containers;
                if (containersToCreate.Any(c => c.Type == AbTestContainerType.ClientRedirect))//если есть контейнер с клиентским редиректом, то больше никаких других контейнеров не добавляем
                    containersToCreate = new[] { containersToCreate.First(c => c.Type == AbTestContainerType.ClientRedirect) };

                foreach (var container in containersToCreate)
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
                    var containerCreateFields = new Dictionary<string, string>
                    {
                        [containerContent.ContentAttributes.First(a => a.NetName == "ParentTest").ColumnName] = testId,
                        [containerContent.ContentAttributes.First(a => a.NetName == "Description").ColumnName] = container.Description,
                        [containerContent.ContentAttributes.First(a => a.NetName == "AllowedUrlPatterns").ColumnName] = container.AllowedUrlPatterns,
                        [containerContent.ContentAttributes.First(a => a.NetName == "DeniedUrlPatterns").ColumnName] = container.DeniedUrlPatterns,
                        [containerContent.ContentAttributes.First(a => a.NetName == "Domain").ColumnName] = container.Domain,
                        [containerContent.ContentAttributes.First(a => a.NetName == "Precondition").ColumnName] = container.Precondition,
                        [containerContent.ContentAttributes.First(a => a.NetName == "Arguments").ColumnName] = container.Arguments,
                        [containerContent.ContentAttributes.First(a => a.NetName == "Type").ColumnName] = containerTypeId.ToString()
                    };

                    _dbConnector.MassUpdate(containerContent.ContentId, new[] { containerCreateFields }, GetUserId());

                    var baseContainerId = containerCreateFields[SystemColumnNames.Id];

                    if (container.Type == AbTestContainerType.Script)
                    {
                        var scriptContainerCreateFields = new Dictionary<string, string>
                        {
                            [scriptContainerContent.ContentAttributes.First(a => a.NetName == "BaseContainer").ColumnName] = baseContainerId
                        };

                        _dbConnector.MassUpdate(scriptContainerContent.ContentId, new[] { scriptContainerCreateFields }, GetUserId());

                        var containerId = scriptContainerCreateFields[SystemColumnNames.Id];

                        var scriptVariants = new List<Dictionary<string, string>>();
                        var versionNumber = 0;
                        foreach (var variant in container.Variants)
                        {
                            if (!String.IsNullOrWhiteSpace(variant))
                            { 
                                scriptVariants.Add(new Dictionary<string, string>
                                {
                                    [scriptContent.ContentAttributes.First(a => a.NetName == "Container").ColumnName] = containerId,
                                    [scriptContent.ContentAttributes.First(a => a.NetName == "VersionNumber").ColumnName] = versionNumber.ToString(),
                                    [scriptContent.ContentAttributes.First(a => a.NetName == "ScriptText").ColumnName] = variant,
                                });
                            }
                            versionNumber++;
                        }

                        _dbConnector.MassUpdate(scriptContent.ContentId, scriptVariants, GetUserId());
                    }
                    else if (container.Type == AbTestContainerType.ClientRedirect)
                    {
                        var redirectContainerCreateFields = new Dictionary<string, string>
                        {
                            [redirectContainerContent.ContentAttributes.First(a => a.NetName == "BaseContainer").ColumnName] = baseContainerId
                        };

                        _dbConnector.MassUpdate(redirectContainerContent.ContentId, new[] { redirectContainerCreateFields }, GetUserId());

                        var containerId = redirectContainerCreateFields[SystemColumnNames.Id];

                        var redirectVariants = new List<Dictionary<string, string>>();
                        var versionNumber = 0;
                        foreach (var variant in container.Variants)
                        {
                            if (!String.IsNullOrWhiteSpace(variant))
                            {
                                redirectVariants.Add(new Dictionary<string, string>
                                {
                                    [redirectContent.ContentAttributes.First(a => a.NetName == "Container").ColumnName] = containerId,
                                    [redirectContent.ContentAttributes.First(a => a.NetName == "VersionNumber").ColumnName] = versionNumber.ToString(),
                                    [redirectContent.ContentAttributes.First(a => a.NetName == "ScriptText").ColumnName] = variant,
                                });
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

        public string GetPercentageAsString(decimal[] percentage)
        {
            return String.Join(";", percentage.Cast<int>());
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
