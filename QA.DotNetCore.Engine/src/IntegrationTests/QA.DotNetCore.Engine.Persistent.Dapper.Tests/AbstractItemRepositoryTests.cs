using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using QA.DotNetCore.Caching;
using QA.DotNetCore.Engine.Persistent.Dapper.Tests.Infrastructure;
using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using QA.DotNetCore.Engine.QpData.Persistent.Dapper;

namespace QA.DotNetCore.Engine.Persistent.Dapper.Tests;


public class AbstractItemRepositoryTests
{
    private const int BaseContentId = 537;
    private const bool IsStage = true;

    private AbstractItemRepository _repository;
    private MetaInfoRepository _metaRepo;

    [SetUp]
    public void Setup()
    {
        var serviceProvider = Global.CreateMockServiceProviderWithConnection();
        var settings = TestUtils.CreateDefaultCacheSettings();
        var memoryCache = new MemoryCache(Options.Create(new MemoryCacheOptions()));
        var memoryCacheProvider = new VersionedCacheCoreProvider(
            memoryCache,
            new CacheKeyFactoryBase(),
            new MemoryLockFactory(NullLoggerFactory.Instance.CreateLogger<MemoryLockFactory>()),
            Mock.Of<ILogger>()
        );
        _metaRepo = new MetaInfoRepository(serviceProvider, memoryCacheProvider, settings);
        var sqlAnalyzer = new NetNameQueryAnalyzer(_metaRepo);
        _repository = new AbstractItemRepository(serviceProvider, sqlAnalyzer, new StubNamingProvider(),
            memoryCacheProvider, settings);
    }

    [Test]
    public void GetAbstractItemExtensionDataTest()
    {
        const int StartPageId = 741114;
        IDictionary<int, AbstractItemExtensionCollection> sqlExtData = null;
        Assert.That(() =>
        {
            var baseContent = _metaRepo.GetContent("QPAbstractItem", Global.SiteId);
            //получим данные о extension c типом StartPageExtension с id=startPageId
            sqlExtData = _repository.GetAbstractItemExtensionData(547, baseContent, true, false);
        }, Throws.Nothing);
        Assert.That(sqlExtData.TryGetValue(StartPageId, out var startPageData), Is.True);
        var startPageBindings = startPageData?.Get("Bindings", typeof(string));
        Assert.That(startPageBindings, Is.Not.Null);

    }

    [Test]
    public void GetPlainAllAbstractItemsTest()
    {
        Assert.That(() =>
        {
            _repository.GetPlainAllAbstractItems(52, false);
        }, Throws.Nothing);
    }

    [Test]
    public void LoadAbstractItemExtensionTest()
    {
        Assert.That(() =>
        {
            _repository.GetManyToManyData(new[] {741035}, false);
        }, Throws.Nothing);
    }

    [Test]
    public void GetAbstractItemExtensionIds_ContentId_ItemExist()
    {
        const int ExtensionContentId = 547;
        const int ExtensionItemId = 741115;

        var ids = _repository.GetAbstractItemExtensionIds(new[] {ExtensionContentId});

        Assert.That(ids, Does.Contain(ExtensionItemId));
    }

    [Test]
    public void GetAbstractItemExtensionlessData_ExistingItemId_ItemData()
    {
        const int AbstractItemId = 741114;
        var baseContent = new ContentPersistentData {ContentId = BaseContentId};

        var itemsData = _repository.GetAbstractItemExtensionlessData(
            new[] {AbstractItemId},
            baseContent,
            IsStage);

        Assert.That(itemsData, Does.ContainKey(AbstractItemId));
        var abstractItemData = itemsData[AbstractItemId];
        Assert.That(abstractItemData, Is.Not.Null);
        Assert.That(abstractItemData.Keys, Is.Not.Empty);
        Assert.That((int) abstractItemData.Get("content_item_id", typeof(int)), Is.EqualTo(AbstractItemId));
    }

    [Test]
    public void GetAbstractItemExtensionData_ExistingItemId_ItemData()
    {
        const int ExtensionContentId = 547;
        const int AbstractItemId = 741114;
        const int ExtensionItemId = 741115;
        const bool LoadAbstractItemFields = true;
        var baseContent = new ContentPersistentData {ContentId = BaseContentId};

        var itemsData = _repository.GetAbstractItemExtensionData(
            ExtensionContentId,
            baseContent,
            LoadAbstractItemFields,
            IsStage);

        Assert.That(itemsData, Does.ContainKey(AbstractItemId));
        var extensionItemData = itemsData[AbstractItemId];
        Assert.That(extensionItemData, Is.Not.Null);
        Assert.That(extensionItemData.Keys, Is.Not.Empty);
        Assert.That((int) extensionItemData.Get("content_item_id", typeof(int)), Is.EqualTo(ExtensionItemId));
    }

    [Test]
    public void GetManyToManyData_ItemId_HasRelations()
    {
        const int ItemIdWithRelations = 741138;

        var relations = _repository.GetManyToManyData(new[] {ItemIdWithRelations}, IsStage);

        Assert.That(relations, Does.ContainKey(ItemIdWithRelations));
        M2MRelations relation = relations[ItemIdWithRelations];
        Assert.That(relation.GetRelations(), Is.Not.Empty);
    }

    [Test]
    public void GetManyToManyData_ItemId_WithoutRelations()
    {
        const int ItemIdWithoutRelations = 741114;

        var relations = _repository.GetManyToManyData(new[] {ItemIdWithoutRelations}, IsStage);

        Assert.That(relations, Is.Empty);
    }

    [Test]
    public void GetManyToManyDataByContent_ContentId_HasRelations()
    {
        const int ContentId = 538;
        const int ItemIdWithRelations = 741138;

        var itemRelationsByContents = _repository.GetManyToManyDataByContents(
            new[] {ContentId},
            IsStage);

        Assert.That(itemRelationsByContents, Does.ContainKey(ItemIdWithRelations));
        var relation = itemRelationsByContents[ItemIdWithRelations];
        Assert.That(relation.GetRelations(), Is.Not.Empty);
    }

    [Test]
    public void GetManyToManyDataByContent_ContentId_WithoutRelations()
    {
        const int ContentId = 99999;

        var itemRelationsByContents = _repository.GetManyToManyDataByContents(
            new[] {ContentId},
            IsStage);

        Assert.That(itemRelationsByContents, Is.Empty);
    }
}
