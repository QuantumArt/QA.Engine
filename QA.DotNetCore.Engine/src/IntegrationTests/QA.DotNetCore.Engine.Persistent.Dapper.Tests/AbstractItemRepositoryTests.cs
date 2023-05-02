using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;
using Tests.CommonUtils.Xunit.Traits;
using Xunit;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests;

[Category(CategoryType.Integration)]
public class AbstractItemRepositoryTests
{
    private const int BaseContentId = 537;
    private const bool IsStage = true;

    private readonly AbstractItemRepository _repository;
    private readonly MetaInfoRepository _metaRepo;

    public AbstractItemRepositoryTests()
    {
        var serviceProvider = Global.CreateMockServiceProviderWithConnection();
        var settings = TestUtils.CreateDefaultCacheSettings();
        var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        var memoryCacheProvider = new VersionedCacheCoreProvider(memoryCache, Mock.Of<ILogger>());
        _metaRepo = new MetaInfoRepository(serviceProvider, memoryCacheProvider, settings);
        var sqlAnalyzer = new NetNameQueryAnalyzer(_metaRepo);
        _repository = new AbstractItemRepository(serviceProvider, sqlAnalyzer, new StubNamingProvider(), memoryCacheProvider, settings);
    }

    [Fact]
    public void GetAbstractItemExtensionDataTest()
    {
        var exception = Record.Exception(() =>
        {
            const int StartPageId = 741114;

            var baseContent = _metaRepo.GetContent("QPAbstractItem", Global.SiteId);
            //получим данные о extension c типом StartPageExtension с id=startPageId
            var sqlExtData = _repository.GetAbstractItemExtensionData(547, baseContent, true, false);
            Assert.True(sqlExtData.TryGetValue(StartPageId, out var startPageData));
            var startPageBindings = Assert.IsType<string>(startPageData.Get("Bindings", typeof(string)));
            Assert.NotNull(startPageBindings);
        });

        Assert.Null(exception);
    }

    [Fact]
    public void GetPlainAllAbstractItemsTest()
    {
        var exception = Record.Exception(() => _repository.GetPlainAllAbstractItems(52, false));

        Assert.Null(exception);
    }

    [Fact]
    public void LoadAbstractItemExtensionTest()
    {
        var exception = Record.Exception(() => _repository.GetManyToManyData(new[] { 741035 }, false));

        Assert.Null(exception);
    }

    [Fact]
    public void GetAbstractItemExtensionIds_ContentId_ItemExist()
    {
        const int ExtensionContentId = 547;
        const int ExtensionItemId = 741115;

        var ids = _repository.GetAbstractItemExtensionIds(new[] { ExtensionContentId });

        Assert.Contains(ExtensionItemId, ids);
    }

    [Fact]
    public void GetAbstractItemExtensionlessData_ExistingItemId_ItemData()
    {
        const int AbstractItemId = 741114;
        var baseContent = new ContentPersistentData { ContentId = BaseContentId };

        var itemsData = _repository.GetAbstractItemExtensionlessData(
            new[] { AbstractItemId },
            baseContent,
            IsStage);

        var abstractItemData = Assert.Contains(AbstractItemId, itemsData);
        Assert.NotNull(abstractItemData);
        Assert.NotEmpty(abstractItemData.Keys);
        Assert.Equal(AbstractItemId, (int)abstractItemData.Get("content_item_id", typeof(int)));
    }

    [Fact]
    public void GetAbstractItemExtensionData_ExistingItemId_ItemData()
    {
        const int ExtensionContentId = 547;
        const int AbstractItemId = 741114;
        const int ExtensionItemId = 741115;
        const bool LoadAbstractItemFields = true;
        var baseContent = new ContentPersistentData { ContentId = BaseContentId };

        var itemsData = _repository.GetAbstractItemExtensionData(
            ExtensionContentId,
            baseContent,
            LoadAbstractItemFields,
            IsStage);

        var extensionItemData = Assert.Contains(AbstractItemId, itemsData);
        Assert.NotNull(extensionItemData);
        Assert.NotEmpty(extensionItemData.Keys);
        Assert.Equal(ExtensionItemId, (int)extensionItemData.Get("content_item_id", typeof(int)));
    }

    [Fact]
    public void GetManyToManyData_ItemId_HasRelations()
    {
        const int ItemIdWithRelations = 741138;

        var relations = _repository.GetManyToManyData(new[] { ItemIdWithRelations }, IsStage);

        M2MRelations relation = Assert.Contains(ItemIdWithRelations, relations);
        Assert.NotEmpty(relation.GetRelations());
    }

    [Fact]
    public void GetManyToManyData_ItemId_WithoutRelations()
    {
        const int ItemIdWithoutRelations = 741114;

        var relations = _repository.GetManyToManyData(new[] { ItemIdWithoutRelations }, IsStage);

        Assert.Empty(relations);
    }

    [Fact]
    public void GetManyToManyDataByContent_ContentId_HasRelations()
    {
        const int ContentId = 538;
        const int ItemIdWithRelations = 741138;

        var itemRelationsByContents = _repository.GetManyToManyDataByContent(
            new[] { ContentId },
            IsStage);

        var itemRelations = Assert.Single(itemRelationsByContents);
        var relation = Assert.Contains(ItemIdWithRelations, itemRelations);
        Assert.NotEmpty(relation.GetRelations());
    }

    [Fact]
    public void GetManyToManyDataByContent_ContentId_WithoutRelations()
    {
        const int ContentId = 99999;

        var itemRelationsByContents = _repository.GetManyToManyDataByContent(
            new[] { ContentId },
            IsStage);

        var itemRelations = Assert.Single(itemRelationsByContents);
        Assert.Empty(itemRelations);
    }
}
