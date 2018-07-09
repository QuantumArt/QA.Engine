using System.Collections.Generic;
using System.Linq;
using Quantumart.QP8.CodeGeneration.Services;


namespace QA.DemoSite.DAL
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
                    Id = 68516,
                    ContentId = 537,
                    Name = "TitleFormat",
                    MappedName = "TitleFormat_ID",
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
                    Id = 27541,
                    ContentId = 538,
                    Name = "FriendlyDescription",
                    MappedName = "TypeName",
                    LinkId = 0,
                    Type = "String"
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
                    Id = 68541,
                    ContentId = 30741,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 68542,
                    ContentId = 30741,
                    Name = "Date",
                    MappedName = "Date",
                    LinkId = 0,
                    Type = "DateTime"
                },
                new AttributeInfo
                {
                    Id = 68544,
                    ContentId = 30741,
                    Name = "Category",
                    MappedName = "Category",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 68545,
                    ContentId = 30741,
                    Name = "Text",
                    MappedName = "Text",
                    LinkId = 0,
                    Type = "VisualEdit"
                },
                new AttributeInfo
                {
                    Id = 68543,
                    ContentId = 30742,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 68546,
                    ContentId = 537,
                    Name = "Children",
                    MappedName = "Children",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 68547,
                    ContentId = 538,
                    Name = "Items",
                    MappedName = "Items",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 68548,
                    ContentId = 537,
                    Name = "Versions",
                    MappedName = "Versions",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 68549,
                    ContentId = 540,
                    Name = "AbstractItems",
                    MappedName = "AbstractItems",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 68550,
                    ContentId = 30742,
                    Name = "NewsByCategory",
                    MappedName = "NewsByCategory",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 68551,
                    ContentId = 538,
                    Name = "BackwardForAllowedItemDefinitions1",
                    MappedName = "BackwardForAllowedItemDefinitions1",
                    LinkId = 88,
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
                   Attributes = new List<AttributeInfo>(attributesLookup[537]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 538,
                   MappedName = "QPDiscriminator",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[538]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 540,
                   MappedName = "QPCulture",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[540]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 30741,
                   MappedName = "NewsItem",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[30741]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 30742,
                   MappedName = "NewsCategory",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[30742]),
                   IsVirtual = false
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
