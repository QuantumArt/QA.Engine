using System.Collections.Generic;
using System.Linq;
using Quantumart.QP8.CoreCodeGeneration.Services;
using QP.ConfigurationService.Models;

/* place your custom usings here */

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
            schema.Schema.DBType = DatabaseType.Postgres;

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
                    Name = "TypeName",
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
                    Id = 47910,
                    ContentId = 538,
                    Name = "AllowedItemDefinitions",
                    MappedName = "AllowedItemDefinitions",
                    LinkId = 0,
                    Type = "M2O"
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
                    Id = 48204,
                    ContentId = 10663,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48205,
                    ContentId = 10663,
                    Name = "Enabled",
                    MappedName = "Enabled",
                    LinkId = 0,
                    Type = "Boolean"
                },
                new AttributeInfo
                {
                    Id = 48206,
                    ContentId = 10663,
                    Name = "Percentage",
                    MappedName = "Percentage",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48207,
                    ContentId = 10663,
                    Name = "StartDate",
                    MappedName = "StartDate",
                    LinkId = 0,
                    Type = "DateTime"
                },
                new AttributeInfo
                {
                    Id = 48208,
                    ContentId = 10663,
                    Name = "EndDate",
                    MappedName = "EndDate",
                    LinkId = 0,
                    Type = "DateTime"
                },
                new AttributeInfo
                {
                    Id = 48209,
                    ContentId = 10663,
                    Name = "Comment",
                    MappedName = "Comment",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 48212,
                    ContentId = 10663,
                    Name = "ABTestContainers",
                    MappedName = "ABTestContainers",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 48211,
                    ContentId = 10664,
                    Name = "ParentTest",
                    MappedName = "ParentTest",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 48213,
                    ContentId = 10664,
                    Name = "Description",
                    MappedName = "Description",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48214,
                    ContentId = 10664,
                    Name = "AllowedUrlPatterns",
                    MappedName = "AllowedUrlPatterns",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 48215,
                    ContentId = 10664,
                    Name = "DeniedUrlPatterns",
                    MappedName = "DeniedUrlPatterns",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 48216,
                    ContentId = 10664,
                    Name = "Domain",
                    MappedName = "Domain",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48217,
                    ContentId = 10664,
                    Name = "Precondition",
                    MappedName = "Precondition",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 48218,
                    ContentId = 10664,
                    Name = "Arguments",
                    MappedName = "Arguments",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 48220,
                    ContentId = 10664,
                    Name = "Type",
                    MappedName = "Type",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 48225,
                    ContentId = 10665,
                    Name = "Container",
                    MappedName = "Container",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 48228,
                    ContentId = 10665,
                    Name = "Description",
                    MappedName = "Description",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48226,
                    ContentId = 10665,
                    Name = "VersionNumber",
                    MappedName = "VersionNumber",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 48227,
                    ContentId = 10665,
                    Name = "ScriptText",
                    MappedName = "ScriptText",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 48224,
                    ContentId = 10666,
                    Name = "BaseContainer",
                    MappedName = "BaseContainer",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 48229,
                    ContentId = 10666,
                    Name = "Scripts",
                    MappedName = "ScriptsInContainer",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 48230,
                    ContentId = 10667,
                    Name = "BaseContainer",
                    MappedName = "BaseContainer",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 48232,
                    ContentId = 10667,
                    Name = "ClientRedirects",
                    MappedName = "ClientRedirectsInContainer",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 48231,
                    ContentId = 10668,
                    Name = "Container",
                    MappedName = "Container",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 48233,
                    ContentId = 10668,
                    Name = "VersionNumber",
                    MappedName = "VersionNumber",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 48234,
                    ContentId = 10668,
                    Name = "RedirectUrl",
                    MappedName = "RedirectUrl",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 68556,
                    ContentId = 30745,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 68567,
                    ContentId = 30745,
                    Name = "Brief",
                    MappedName = "Brief",
                    LinkId = 0,
                    Type = "Textbox"
                },
                new AttributeInfo
                {
                    Id = 68568,
                    ContentId = 30745,
                    Name = "PostDate",
                    MappedName = "PostDate",
                    LinkId = 0,
                    Type = "Date"
                },
                new AttributeInfo
                {
                    Id = 68569,
                    ContentId = 30745,
                    Name = "Category",
                    MappedName = "Category",
                    LinkId = 0,
                    Type = "O2M"
                },
                new AttributeInfo
                {
                    Id = 68570,
                    ContentId = 30745,
                    Name = "Text",
                    MappedName = "Text",
                    LinkId = 0,
                    Type = "VisualEdit"
                },
                new AttributeInfo
                {
                    Id = 68571,
                    ContentId = 30745,
                    Name = "Tags",
                    MappedName = "Tags",
                    LinkId = 140,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 68572,
                    ContentId = 30745,
                    Name = "Image",
                    MappedName = "Image",
                    LinkId = 0,
                    Type = "Image"
                },
                new AttributeInfo
                {
                    Id = 68573,
                    ContentId = 30745,
                    Name = "YoutubeVideoCode",
                    MappedName = "YoutubeVideoCode",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 68557,
                    ContentId = 30746,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 68576,
                    ContentId = 30746,
                    Name = "SortOrder",
                    MappedName = "SortOrder",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 68558,
                    ContentId = 30747,
                    Name = "Title",
                    MappedName = "Title",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 68561,
                    ContentId = 30749,
                    Name = "Question",
                    MappedName = "Question",
                    LinkId = 0,
                    Type = "String"
                },
                new AttributeInfo
                {
                    Id = 68562,
                    ContentId = 30749,
                    Name = "Answer",
                    MappedName = "Answer",
                    LinkId = 0,
                    Type = "VisualEdit"
                },
                new AttributeInfo
                {
                    Id = 68574,
                    ContentId = 30749,
                    Name = "SortOrder",
                    MappedName = "SortOrder",
                    LinkId = 0,
                    Type = "Numeric"
                },
                new AttributeInfo
                {
                    Id = 68577,
                    ContentId = 537,
                    Name = "Children",
                    MappedName = "Children",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 68578,
                    ContentId = 538,
                    Name = "Items",
                    MappedName = "Items",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 68579,
                    ContentId = 537,
                    Name = "Versions",
                    MappedName = "Versions",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 68580,
                    ContentId = 540,
                    Name = "AbstractItems",
                    MappedName = "AbstractItems",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 68581,
                    ContentId = 538,
                    Name = "AllowDefinition",
                    MappedName = "AllowDefinition",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 68582,
                    ContentId = 10664,
                    Name = "ScriptContainers",
                    MappedName = "ScriptContainers",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 68583,
                    ContentId = 10664,
                    Name = "ClientRedirectContainers",
                    MappedName = "ClientRedirectContainers",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 68584,
                    ContentId = 30746,
                    Name = "PostsInCategory",
                    MappedName = "PostsInCategory",
                    LinkId = 0,
                    Type = "M2O"
                },
                new AttributeInfo
                {
                    Id = 68585,
                    ContentId = 538,
                    Name = "BackwardForAllowedItemDefinitions1",
                    MappedName = "BackwardForAllowedItemDefinitions1",
                    LinkId = 88,
                    Type = "M2M"
                },
                new AttributeInfo
                {
                    Id = 68586,
                    ContentId = 30747,
                    Name = "BackwardForTags",
                    MappedName = "BackwardForTags",
                    LinkId = 140,
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
                   Id = 10609,
                   MappedName = "QPItemDefinitionConstraint",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10609]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 10663,
                   MappedName = "AbTest",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10663]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 10664,
                   MappedName = "AbTestBaseContainer",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10664]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 10665,
                   MappedName = "AbTestScript",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10665]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 10666,
                   MappedName = "AbTestScriptContainer",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10666]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 10667,
                   MappedName = "AbTestClientRedirectContainer",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10667]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 10668,
                   MappedName = "AbTestClientRedirect",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[10668]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 30745,
                   MappedName = "BlogPost",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[30745]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 30746,
                   MappedName = "BlogCategory",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[30746]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 30747,
                   MappedName = "BlogTag",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[30747]),
                   IsVirtual = false
                },
                new ContentInfo
                {
                   Id = 30749,
                   MappedName = "FaqItem",
                   UseDefaultFiltration = true,
                   Attributes = new List<AttributeInfo>(attributesLookup[30749]),
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
