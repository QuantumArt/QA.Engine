using System.Collections.Generic;
using System.Linq;
using Quantumart.QP8.CodeGeneration.Services;


namespace Demo.DAL
{
    public class StaticSchemaProvider : ISchemaProvider
    {
       public StaticSchemaProvider()
       {
       }

        #region ISchemaProvider implementation
        public ModelReader GetSchema()
        {
            var schema = new ModelReader();

            schema.Schema.SiteName = "main_site";
            schema.Schema.ReplaceUrls = true;

            schema.Attributes = new List<AttributeInfo>
            {
                new AttributeInfo
                {
                    Id = 27505,
                    ContentId = 537,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27507,
                    ContentId = 537,
                    Name = "Name",
                    MappedName = "Name",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27508,
                    ContentId = 537,
                    Name = "Parent",
                    MappedName = "Parent",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 27509,
                    ContentId = 537,
                    Name = "IsVisible",
                    MappedName = "IsVisible",
                    LinkId = 0,
                    Type = "Boolean"
                },
                new AttributeInfo
                {
                    Id = 27510,
                    ContentId = 537,
                    Name = "IsPage",
                    MappedName = "IsPage",
                    LinkId = 0,
                    Type = "Boolean"
                },
                new AttributeInfo
                {
                    Id = 27529,
                    ContentId = 537,
                    Name = "Regions",
                    MappedName = "Regions",
                    LinkId = 87,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 27512,
                    ContentId = 537,
                    Name = "ZoneName",
                    MappedName = "ZoneName",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27513,
                    ContentId = 537,
                    Name = "AllowedUrlPatterns",
                    MappedName = "AllowedUrlPatterns",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 27514,
                    ContentId = 537,
                    Name = "DeniedUrlPatterns",
                    MappedName = "DeniedUrlPatterns",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 27515,
                    ContentId = 537,
                    Name = "Description",
                    MappedName = "Description",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27516,
                    ContentId = 537,
                    Name = "Discriminator",
                    MappedName = "Discriminator",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 27520,
                    ContentId = 537,
                    Name = "VersionOf",
                    MappedName = "VersionOf",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 27521,
                    ContentId = 537,
                    Name = "Culture",
                    MappedName = "Culture",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 27532,
                    ContentId = 537,
                    Name = "TitleFormat",
                    MappedName = "TitleFormat",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 27533,
                    ContentId = 537,
                    Name = "Keywords",
                    MappedName = "Keywords",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27534,
                    ContentId = 537,
                    Name = "MetaDescription",
                    MappedName = "MetaDescription",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27535,
                    ContentId = 537,
                    Name = "Tags",
                    MappedName = "Tags",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 27536,
                    ContentId = 537,
                    Name = "IsInSiteMap",
                    MappedName = "IsInSiteMap",
                    LinkId = 0,
                    Type = "Boolean"
                },
                new AttributeInfo
                {
                    Id = 27537,
                    ContentId = 537,
                    Name = "IndexOrder",
                    MappedName = "IndexOrder",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 27538,
                    ContentId = 537,
                    Name = "ExtensionId",
                    MappedName = "ExtensionId",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 37906,
                    ContentId = 537,
                    Name = "ContentId",
                    MappedName = "ContentId",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 27506,
                    ContentId = 538,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27539,
                    ContentId = 538,
                    Name = "Name",
                    MappedName = "Name",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27540,
                    ContentId = 538,
                    Name = "PreferredContentId",
                    MappedName = "PreferredContentId",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 27542,
                    ContentId = 538,
                    Name = "CategoryName",
                    MappedName = "CategoryName",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27543,
                    ContentId = 538,
                    Name = "Description",
                    MappedName = "Description",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27544,
                    ContentId = 538,
                    Name = "IconUrl",
                    MappedName = "IconUrl",
                    LinkId = 0,
                    Type = "Image"
                },
                new AttributeInfo
                {
                    Id = 27545,
                    ContentId = 538,
                    Name = "IsPage",
                    MappedName = "IsPage",
                    LinkId = 0,
                    Type = "Boolean"
                },
                new AttributeInfo
                {
                    Id = 27546,
                    ContentId = 538,
                    Name = "AllowedZones",
                    MappedName = "AllowedZones",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27552,
                    ContentId = 538,
                    Name = "AllowedItemDefinitions1",
                    MappedName = "AllowedItemDefinitions1",
                    LinkId = 88,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 27553,
                    ContentId = 538,
                    Name = "FilterPartByUrl",
                    MappedName = "FilterPartByUrl",
                    LinkId = 0,
                    Type = "Boolean"
                },
                new AttributeInfo
                {
                    Id = 47910,
                    ContentId = 538,
                    Name = "AllowedItemDefinitions",
                    MappedName = "AllowedItemDefinitions",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 27523,
                    ContentId = 539,
                    Name = "Parent",
                    MappedName = "Parent",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 27511,
                    ContentId = 539,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27524,
                    ContentId = 539,
                    Name = "Url",
                    MappedName = "Url",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27517,
                    ContentId = 540,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27518,
                    ContentId = 540,
                    Name = "Name",
                    MappedName = "Name",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27519,
                    ContentId = 540,
                    Name = "Icon",
                    MappedName = "Icon",
                    LinkId = 0,
                    Type = "Image"
                },
                new AttributeInfo
                {
                    Id = 27530,
                    ContentId = 541,
                    Name = "Value",
                    MappedName = "Value",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27531,
                    ContentId = 541,
                    Name = "Description",
                    MappedName = "Description",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 27525,
                    ContentId = 542,
                    Name = "ParentId",
                    MappedName = "ParentId",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 27527,
                    ContentId = 542,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27528,
                    ContentId = 542,
                    Name = "Alias",
                    MappedName = "Alias",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27555,
                    ContentId = 545,
                    Name = "Trail",
                    MappedName = "Trail",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 27556,
                    ContentId = 545,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 27557,
                    ContentId = 545,
                    Name = "Name",
                    MappedName = "Name",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 37743,
                    ContentId = 578,
                    Name = "SettingKey",
                    MappedName = "SettingKey",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 37745,
                    ContentId = 578,
                    Name = "SettingType",
                    MappedName = "SettingType",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 37744,
                    ContentId = 578,
                    Name = "SettingValue",
                    MappedName = "SettingValue",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 37883,
                    ContentId = 602,
                    Name = "Key",
                    MappedName = "Key",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 37884,
                    ContentId = 602,
                    Name = "Value",
                    MappedName = "Value",
                    LinkId = 0,
                    Type = "VisualEdit"
                },
                new AttributeInfo
                {
                    Id = 37886,
                    ContentId = 603,
                    Name = "ParentId",
                    MappedName = "Parent",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 37889,
                    ContentId = 603,
                    Name = "Regions",
                    MappedName = "Regions",
                    LinkId = 114,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 37891,
                    ContentId = 603,
                    Name = "Value",
                    MappedName = "Value",
                    LinkId = 0,
                    Type = "VisualEdit"
                },
                new AttributeInfo
                {
                    Id = 47908,
                    ContentId = 10609,
                    Name = "Target",
                    MappedName = "Target",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 47909,
                    ContentId = 10609,
                    Name = "Source",
                    MappedName = "Source",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 47915,
                    ContentId = 10610,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48012,
                    ContentId = 10610,
                    Name = "TitleForDetails",
                    MappedName = "TitleForDetails",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48013,
                    ContentId = 10610,
                    Name = "TransliterationTitle",
                    MappedName = "TransliterationTitle",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 47970,
                    ContentId = 10610,
                    Name = "Alias",
                    MappedName = "PublishDate",
                    LinkId = 0,
                    Type = "DateTime"
                },
                new AttributeInfo
                {
                    Id = 47971,
                    ContentId = 10610,
                    Name = "ParentManyToMany",
                    MappedName = "Groups",
                    LinkId = 118,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 47918,
                    ContentId = 10610,
                    Name = "Anounce",
                    MappedName = "Anounce",
                    LinkId = 0,
                    Type = "VisualEdit"
                },
                new AttributeInfo
                {
                    Id = 47919,
                    ContentId = 10610,
                    Name = "Text",
                    MappedName = "Text",
                    LinkId = 0,
                    Type = "VisualEdit"
                },
                new AttributeInfo
                {
                    Id = 47923,
                    ContentId = 10610,
                    Name = "Regions",
                    MappedName = "Regions",
                    LinkId = 115,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 47954,
                    ContentId = 10610,
                    Name = "Order",
                    MappedName = "Order",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 47959,
                    ContentId = 10610,
                    Name = "MainImage",
                    MappedName = "MainImage",
                    LinkId = 0,
                    Type = "Image"
                },
                new AttributeInfo
                {
                    Id = 47963,
                    ContentId = 10610,
                    Name = "PublishSMI",
                    MappedName = "PublishSMI",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 47964,
                    ContentId = 10610,
                    Name = "Keywords",
                    MappedName = "Keywords",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 47965,
                    ContentId = 10610,
                    Name = "MetaDescription",
                    MappedName = "MetaDescription",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 47990,
                    ContentId = 10610,
                    Name = "Rubrics",
                    MappedName = "Rubrics",
                    LinkId = 121,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 47958,
                    ContentId = 10610,
                    Name = "CommonAlias",
                    MappedName = "Alias",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 47992,
                    ContentId = 10610,
                    Name = "TitleFormat",
                    MappedName = "TitleFormat",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 48014,
                    ContentId = 10610,
                    Name = "SmallImage",
                    MappedName = "SmallImage",
                    LinkId = 0,
                    Type = "Image"
                },
                new AttributeInfo
                {
                    Id = 47924,
                    ContentId = 10612,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 47986,
                    ContentId = 10627,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 47987,
                    ContentId = 10627,
                    Name = "RubricId",
                    MappedName = "RubricId",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 48010,
                    ContentId = 10627,
                    Name = "Order",
                    MappedName = "Order",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 48011,
                    ContentId = 10627,
                    Name = "Alias",
                    MappedName = "Alias",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48098,
                    ContentId = 10644,
                    Name = "Order",
                    MappedName = "Order",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 48099,
                    ContentId = 10644,
                    Name = "Text",
                    MappedName = "Text",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 48100,
                    ContentId = 10644,
                    Name = "Url",
                    MappedName = "Url",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48103,
                    ContentId = 10646,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48104,
                    ContentId = 10646,
                    Name = "Order",
                    MappedName = "Order",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 48105,
                    ContentId = 10646,
                    Name = "LT1Text",
                    MappedName = "LT1Text",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 48106,
                    ContentId = 10646,
                    Name = "LT1URL",
                    MappedName = "LT1URL",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48107,
                    ContentId = 10646,
                    Name = "LB2Text",
                    MappedName = "LB2Text",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 48108,
                    ContentId = 10646,
                    Name = "LB2URL",
                    MappedName = "LB2URL",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48109,
                    ContentId = 10646,
                    Name = "RT3Text",
                    MappedName = "RT3Text",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 48110,
                    ContentId = 10646,
                    Name = "RT3URL",
                    MappedName = "RT3URL",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48111,
                    ContentId = 10646,
                    Name = "RB4Text",
                    MappedName = "RB4Text",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 48112,
                    ContentId = 10646,
                    Name = "RB4URL",
                    MappedName = "RB4URL",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48117,
                    ContentId = 10648,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48118,
                    ContentId = 10648,
                    Name = "Alias",
                    MappedName = "Alias",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48119,
                    ContentId = 10648,
                    Name = "Regions",
                    MappedName = "Regions",
                    LinkId = 126,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 48121,
                    ContentId = 10648,
                    Name = "Description",
                    MappedName = "Description",
                    LinkId = 0,
                    Type = "VisualEdit"
                },
                new AttributeInfo
                {
                    Id = 48122,
                    ContentId = 10648,
                    Name = "Text",
                    MappedName = "Text",
                    LinkId = 0,
                    Type = "VisualEdit"
                },
                new AttributeInfo
                {
                    Id = 48123,
                    ContentId = 10648,
                    Name = "ShowUntil",
                    MappedName = "ShowUntil",
                    LinkId = 0,
                    Type = "DateTime"
                },
                new AttributeInfo
                {
                    Id = 48132,
                    ContentId = 10651,
                    Name = "PostTitle",
                    MappedName = "PostTitle",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48134,
                    ContentId = 10651,
                    Name = "PostDescription",
                    MappedName = "PostDescription",
                    LinkId = 0,
                    Type = "VisualEdit"
                },
                new AttributeInfo
                {
                    Id = 48133,
                    ContentId = 10651,
                    Name = "PostText",
                    MappedName = "PostText",
                    LinkId = 0,
                    Type = "VisualEdit"
                },
                new AttributeInfo
                {
                    Id = 48136,
                    ContentId = 10651,
                    Name = "PostImgDescription",
                    MappedName = "PostImgDescription",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48135,
                    ContentId = 10651,
                    Name = "PostImage",
                    MappedName = "PostImage",
                    LinkId = 0,
                    Type = "Image"
                },
                new AttributeInfo
                {
                    Id = 48137,
                    ContentId = 537,
                    Name = "Children",
                    MappedName = "Children",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 48138,
                    ContentId = 538,
                    Name = "Items",
                    MappedName = "Items",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 48139,
                    ContentId = 537,
                    Name = "Versions",
                    MappedName = "Versions",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 48140,
                    ContentId = 540,
                    Name = "AbstractItems",
                    MappedName = "AbstractItems",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 48141,
                    ContentId = 541,
                    Name = "Item",
                    MappedName = "Item",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 48142,
                    ContentId = 539,
                    Name = "Children",
                    MappedName = "Children",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 48143,
                    ContentId = 602,
                    Name = "QpRegionTagValues",
                    MappedName = "QpRegionTagValues",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 48144,
                    ContentId = 538,
                    Name = "AllowDefinition",
                    MappedName = "AllowDefinition",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 48145,
                    ContentId = 10624,
                    Name = "News",
                    MappedName = "News",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 48146,
                    ContentId = 541,
                    Name = "News",
                    MappedName = "News",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 48147,
                    ContentId = 542,
                    Name = "BackwardForRegions",
                    MappedName = "BackwardForRegions",
                    LinkId = 87,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 48148,
                    ContentId = 10627,
                    Name = "BackwardForRubrics",
                    MappedName = "BackwardForRubrics",
                    LinkId = 121,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 48149,
                    ContentId = 538,
                    Name = "BackwardForAllowedItemDefinitions1",
                    MappedName = "BackwardForAllowedItemDefinitions1",
                    LinkId = 88,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 48150,
                    ContentId = 539,
                    Name = "BackwardForRegions",
                    MappedName = "BackwardForRegions",
                    LinkId = 114,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 48151,
                    ContentId = 10612,
                    Name = "BackwardForGroups",
                    MappedName = "BackwardForGroups",
                    LinkId = 118,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 48152,
                    ContentId = 539,
                    Name = "BackwardForRegions1",
                    MappedName = "BackwardForRegions1",
                    LinkId = 115,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 48153,
                    ContentId = 539,
                    Name = "BackwardForRegions2",
                    MappedName = "BackwardForRegions2",
                    LinkId = 126,
                    Type = "M2M"
                },
            };

            var attributesLookup = schema.Attributes.ToLookup(a => a.ContentId, a => a);

            schema.Contents = new List<ContentInfo>
            {
                new ContentInfo
                {
                   Id = 537,
                   MappedName = "QPAbstractItem",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[537])
                },
                new ContentInfo
                {
                   Id = 538,
                   MappedName = "QPDiscriminator",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[538])
                },
                new ContentInfo
                {
                   Id = 539,
                   MappedName = "MarketingRegion",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[539])
                },
                new ContentInfo
                {
                   Id = 540,
                   MappedName = "QPCulture",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[540])
                },
                new ContentInfo
                {
                   Id = 541,
                   MappedName = "ItemTitleFormat",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[541])
                },
                new ContentInfo
                {
                   Id = 542,
                   MappedName = "QPRegion",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[542])
                },
                new ContentInfo
                {
                   Id = 545,
                   MappedName = "TrailedAbstractItem",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[545])
                },
                new ContentInfo
                {
                   Id = 578,
                   MappedName = "SiteSetting",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[578])
                },
                new ContentInfo
                {
                   Id = 602,
                   MappedName = "QpRegionTag",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[602])
                },
                new ContentInfo
                {
                   Id = 603,
                   MappedName = "QpRegionTagValue",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[603])
                },
                new ContentInfo
                {
                   Id = 10609,
                   MappedName = "QPItemDefinitionConstraint",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10609])
                },
                new ContentInfo
                {
                   Id = 10610,
                   MappedName = "News",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10610])
                },
                new ContentInfo
                {
                   Id = 10612,
                   MappedName = "NewsGroup",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10612])
                },
                new ContentInfo
                {
                   Id = 10624,
                   MappedName = "QpSMI",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10624])
                },
                new ContentInfo
                {
                   Id = 10627,
                   MappedName = "QpNewsRubric",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10627])
                },
                new ContentInfo
                {
                   Id = 10644,
                   MappedName = "PopularServicesValue",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10644])
                },
                new ContentInfo
                {
                   Id = 10646,
                   MappedName = "MainSectionsValue",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10646])
                },
                new ContentInfo
                {
                   Id = 10648,
                   MappedName = "QpAction",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10648])
                },
                new ContentInfo
                {
                   Id = 10651,
                   MappedName = "QpBlog",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10651])
                },
            };

            schema.Contents.ForEach(c => c.Attributes.ForEach(a => a.Content = c));

            return schema;
        }

        public object GetCacheKey()
        {
            return null;
        }
        #endregion
    }
}
