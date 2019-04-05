using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using Quantumart.QP8.CoreCodeGeneration.Services;
using Quantumart.QP8.EntityFrameworkCore;
/* place your custom usings here */

namespace QA.DemoSite.DAL
{
    public class MappingConfigurator : MappingConfiguratorBase
    {
        public MappingConfigurator(ContentAccess contentAccess, ISchemaProvider schemaProvider)
            : base(contentAccess, schemaProvider)
        {
        }

        public override void OnModelCreating(ModelBuilder modelBuilder)
        {
        base.OnModelCreating(modelBuilder);

            #region QPAbstractItem mappings
            modelBuilder.Entity<QPAbstractItem>()
                .ToTable(GetTableName("QPAbstractItem"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<QPAbstractItem>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QPAbstractItem>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.Title)
                .HasColumnName(GetFieldName("QPAbstractItem", "Title"));
            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.Name)
                .HasColumnName(GetFieldName("QPAbstractItem", "Name"));
            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.ZoneName)
                .HasColumnName(GetFieldName("QPAbstractItem", "ZoneName"));
            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.AllowedUrlPatterns)
                .HasColumnName(GetFieldName("QPAbstractItem", "AllowedUrlPatterns"));
            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.DeniedUrlPatterns)
                .HasColumnName(GetFieldName("QPAbstractItem", "DeniedUrlPatterns"));
            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.Description)
                .HasColumnName(GetFieldName("QPAbstractItem", "Description"));
            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.Keywords)
                .HasColumnName(GetFieldName("QPAbstractItem", "Keywords"));
            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.MetaDescription)
                .HasColumnName(GetFieldName("QPAbstractItem", "MetaDescription"));
            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.Tags)
                .HasColumnName(GetFieldName("QPAbstractItem", "Tags"));
            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.TitleFormat_ID)
                .HasColumnName(GetFieldName("QPAbstractItem", "TitleFormat_ID"));
            modelBuilder.Entity<QPAbstractItem>()
                .HasOne<QPAbstractItem>(mp => mp.Parent)
                .WithMany(mp => mp.Children)
                .HasForeignKey(fp => fp.Parent_ID);

            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.Parent_ID)
                .HasColumnName(GetFieldName("QPAbstractItem", "Parent"));
            modelBuilder.Entity<QPAbstractItem>()
                .HasOne<QPDiscriminator>(mp => mp.Discriminator)
                .WithMany(mp => mp.Items)
                .HasForeignKey(fp => fp.Discriminator_ID);

            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.Discriminator_ID)
                .HasColumnName(GetFieldName("QPAbstractItem", "Discriminator"));
            modelBuilder.Entity<QPAbstractItem>()
                .HasOne<QPAbstractItem>(mp => mp.VersionOf)
                .WithMany(mp => mp.Versions)
                .HasForeignKey(fp => fp.VersionOf_ID);

            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.VersionOf_ID)
                .HasColumnName(GetFieldName("QPAbstractItem", "VersionOf"));
            modelBuilder.Entity<QPAbstractItem>()
                .HasOne<QPCulture>(mp => mp.Culture)
                .WithMany(mp => mp.AbstractItems)
                .HasForeignKey(fp => fp.Culture_ID);

            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.Culture_ID)
                .HasColumnName(GetFieldName("QPAbstractItem", "Culture"));
 
            #endregion

            #region QPDiscriminator mappings
            modelBuilder.Entity<QPDiscriminator>()
                .ToTable(GetTableName("QPDiscriminator"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<QPDiscriminator>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<QPDiscriminator>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QPDiscriminator>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QPDiscriminator>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<QPDiscriminator>()
                .Property(x => x.Title)
                .HasColumnName(GetFieldName("QPDiscriminator", "Title"));
            modelBuilder.Entity<QPDiscriminator>()
                .Property(x => x.Name)
                .HasColumnName(GetFieldName("QPDiscriminator", "Name"));
            modelBuilder.Entity<QPDiscriminator>()
                .Property(x => x.TypeName)
                .HasColumnName(GetFieldName("QPDiscriminator", "TypeName"));
            modelBuilder.Entity<QPDiscriminator>()
                .Property(x => x.CategoryName)
                .HasColumnName(GetFieldName("QPDiscriminator", "CategoryName"));
            modelBuilder.Entity<QPDiscriminator>()
                .Property(x => x.Description)
                .HasColumnName(GetFieldName("QPDiscriminator", "Description"));
            modelBuilder.Entity<QPDiscriminator>()
                .Property(x => x.AllowedZones)
                .HasColumnName(GetFieldName("QPDiscriminator", "AllowedZones"));

             modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForAllowedItemDefinitions1>()
                .ToTable(GetLinkTableName("QPDiscriminator", "AllowedItemDefinitions1"));

            modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForAllowedItemDefinitions1>().Property(e => e.QPDiscriminatorItemId).HasColumnName("id");
            modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForAllowedItemDefinitions1>().Property(e => e.QPDiscriminatorLinkedItemId).HasColumnName("linked_id");
            modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForAllowedItemDefinitions1>().HasKey(ug => new { ug.QPDiscriminatorItemId, ug.QPDiscriminatorLinkedItemId });

            modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForAllowedItemDefinitions1>()
                .HasOne(bc => bc.QPDiscriminatorItem)
                .WithMany(b => b.AllowedItemDefinitions1)
                .HasForeignKey(bc => bc.QPDiscriminatorItemId);

            modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForAllowedItemDefinitions1>()
                .HasOne(bc => bc.QPDiscriminatorLinkedItem)
                .WithMany();
			modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForAllowedItemDefinitions1>().Ignore(x=>x.Id);
			modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForAllowedItemDefinitions1>().Ignore(x=>x.LinkId);
			modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForAllowedItemDefinitions1>().Ignore(x=>x.LinkedItemId);
			modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForAllowedItemDefinitions1>().Ignore(x=>x.Item);
			modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForAllowedItemDefinitions1>().Ignore(x=>x.LinkedItem);

			 modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForBackwardForAllowedItemDefinitions1>()
                .ToTable(GetReversedLinkTableName("QPDiscriminator", "AllowedItemDefinitions1"));
      

			modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForBackwardForAllowedItemDefinitions1>().Property(e => e.QPDiscriminatorLinkedItemId).HasColumnName("id");
			modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForBackwardForAllowedItemDefinitions1>().Property(e => e.QPDiscriminatorItemId).HasColumnName("linked_id");
           
			modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForBackwardForAllowedItemDefinitions1>().HasKey(ug => new { ug.QPDiscriminatorLinkedItemId, ug.QPDiscriminatorItemId });
            
			 modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForBackwardForAllowedItemDefinitions1>()
                .HasOne(bc => bc.QPDiscriminatorItem)
                .WithMany(b => b.BackwardForAllowedItemDefinitions1)
                .HasForeignKey(bc => bc.QPDiscriminatorItemId);

            modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForBackwardForAllowedItemDefinitions1>()
                .HasOne(bc => bc.QPDiscriminatorLinkedItem)
                .WithMany();

			modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForBackwardForAllowedItemDefinitions1>().Ignore(x=>x.Id);
			modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForBackwardForAllowedItemDefinitions1>().Ignore(x=>x.LinkId);
			modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForBackwardForAllowedItemDefinitions1>().Ignore(x=>x.LinkedItemId);
			modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForBackwardForAllowedItemDefinitions1>().Ignore(x=>x.Item);
			modelBuilder.Entity<QPDiscriminator2QPDiscriminatorForBackwardForAllowedItemDefinitions1>().Ignore(x=>x.LinkedItem);
            modelBuilder.Entity<QPDiscriminator>().Ignore(p => p.IconUrlUrl);
            modelBuilder.Entity<QPDiscriminator>().Ignore(p => p.IconUrlUploadPath);
 
            #endregion

            #region QPCulture mappings
            modelBuilder.Entity<QPCulture>()
                .ToTable(GetTableName("QPCulture"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<QPCulture>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<QPCulture>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QPCulture>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QPCulture>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<QPCulture>()
                .Property(x => x.Title)
                .HasColumnName(GetFieldName("QPCulture", "Title"));
            modelBuilder.Entity<QPCulture>()
                .Property(x => x.Name)
                .HasColumnName(GetFieldName("QPCulture", "Name"));
            modelBuilder.Entity<QPCulture>().Ignore(p => p.IconUrl);
            modelBuilder.Entity<QPCulture>().Ignore(p => p.IconUploadPath);
 
            #endregion

            #region QPItemDefinitionConstraint mappings
            modelBuilder.Entity<QPItemDefinitionConstraint>()
                .ToTable(GetTableName("QPItemDefinitionConstraint"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<QPItemDefinitionConstraint>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<QPItemDefinitionConstraint>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QPItemDefinitionConstraint>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QPItemDefinitionConstraint>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<QPItemDefinitionConstraint>()
                .HasOne<QPDiscriminator>(mp => mp.Target)
                .WithMany(mp => mp.AllowDefinition)
                .HasForeignKey(fp => fp.Target_ID);

            modelBuilder.Entity<QPItemDefinitionConstraint>()
                .Property(x => x.Target_ID)
                .HasColumnName(GetFieldName("QPItemDefinitionConstraint", "Target"));
            modelBuilder.Entity<QPItemDefinitionConstraint>()
                .HasOne<QPDiscriminator>(mp => mp.Source)
                .WithMany(mp => mp.AllowedItemDefinitions)
                .HasForeignKey(fp => fp.Source_ID);

            modelBuilder.Entity<QPItemDefinitionConstraint>()
                .Property(x => x.Source_ID)
                .HasColumnName(GetFieldName("QPItemDefinitionConstraint", "Source"));
 
            #endregion

            #region AbTest mappings
            modelBuilder.Entity<AbTest>()
                .ToTable(GetTableName("AbTest"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<AbTest>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<AbTest>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<AbTest>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<AbTest>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<AbTest>()
                .Property(x => x.Title)
                .HasColumnName(GetFieldName("AbTest", "Title"));
            modelBuilder.Entity<AbTest>()
                .Property(x => x.Percentage)
                .HasColumnName(GetFieldName("AbTest", "Percentage"));
            modelBuilder.Entity<AbTest>()
                .Property(x => x.Comment)
                .HasColumnName(GetFieldName("AbTest", "Comment"));
 
            #endregion

            #region AbTestBaseContainer mappings
            modelBuilder.Entity<AbTestBaseContainer>()
                .ToTable(GetTableName("AbTestBaseContainer"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<AbTestBaseContainer>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<AbTestBaseContainer>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<AbTestBaseContainer>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<AbTestBaseContainer>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<AbTestBaseContainer>()
                .Property(x => x.Description)
                .HasColumnName(GetFieldName("AbTestBaseContainer", "Description"));
            modelBuilder.Entity<AbTestBaseContainer>()
                .Property(x => x.AllowedUrlPatterns)
                .HasColumnName(GetFieldName("AbTestBaseContainer", "AllowedUrlPatterns"));
            modelBuilder.Entity<AbTestBaseContainer>()
                .Property(x => x.DeniedUrlPatterns)
                .HasColumnName(GetFieldName("AbTestBaseContainer", "DeniedUrlPatterns"));
            modelBuilder.Entity<AbTestBaseContainer>()
                .Property(x => x.Domain)
                .HasColumnName(GetFieldName("AbTestBaseContainer", "Domain"));
            modelBuilder.Entity<AbTestBaseContainer>()
                .Property(x => x.Precondition)
                .HasColumnName(GetFieldName("AbTestBaseContainer", "Precondition"));
            modelBuilder.Entity<AbTestBaseContainer>()
                .Property(x => x.Arguments)
                .HasColumnName(GetFieldName("AbTestBaseContainer", "Arguments"));
            modelBuilder.Entity<AbTestBaseContainer>()
                .HasOne<AbTest>(mp => mp.ParentTest)
                .WithMany(mp => mp.ABTestContainers)
                .HasForeignKey(fp => fp.ParentTest_ID);

            modelBuilder.Entity<AbTestBaseContainer>()
                .Property(x => x.ParentTest_ID)
                .HasColumnName(GetFieldName("AbTestBaseContainer", "ParentTest"));
 
            #endregion

            #region AbTestScript mappings
            modelBuilder.Entity<AbTestScript>()
                .ToTable(GetTableName("AbTestScript"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<AbTestScript>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<AbTestScript>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<AbTestScript>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<AbTestScript>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<AbTestScript>()
                .Property(x => x.Description)
                .HasColumnName(GetFieldName("AbTestScript", "Description"));
            modelBuilder.Entity<AbTestScript>()
                .Property(x => x.ScriptText)
                .HasColumnName(GetFieldName("AbTestScript", "ScriptText"));
            modelBuilder.Entity<AbTestScript>()
                .HasOne<AbTestScriptContainer>(mp => mp.Container)
                .WithMany(mp => mp.ScriptsInContainer)
                .HasForeignKey(fp => fp.Container_ID);

            modelBuilder.Entity<AbTestScript>()
                .Property(x => x.Container_ID)
                .HasColumnName(GetFieldName("AbTestScript", "Container"));
 
            #endregion

            #region AbTestScriptContainer mappings
            modelBuilder.Entity<AbTestScriptContainer>()
                .ToTable(GetTableName("AbTestScriptContainer"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<AbTestScriptContainer>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<AbTestScriptContainer>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<AbTestScriptContainer>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<AbTestScriptContainer>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<AbTestScriptContainer>()
                .HasOne<AbTestBaseContainer>(mp => mp.BaseContainer)
                .WithMany(mp => mp.ScriptContainers)
                .HasForeignKey(fp => fp.BaseContainer_ID);

            modelBuilder.Entity<AbTestScriptContainer>()
                .Property(x => x.BaseContainer_ID)
                .HasColumnName(GetFieldName("AbTestScriptContainer", "BaseContainer"));
 
            #endregion

            #region AbTestClientRedirectContainer mappings
            modelBuilder.Entity<AbTestClientRedirectContainer>()
                .ToTable(GetTableName("AbTestClientRedirectContainer"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<AbTestClientRedirectContainer>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<AbTestClientRedirectContainer>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<AbTestClientRedirectContainer>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<AbTestClientRedirectContainer>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<AbTestClientRedirectContainer>()
                .HasOne<AbTestBaseContainer>(mp => mp.BaseContainer)
                .WithMany(mp => mp.ClientRedirectContainers)
                .HasForeignKey(fp => fp.BaseContainer_ID);

            modelBuilder.Entity<AbTestClientRedirectContainer>()
                .Property(x => x.BaseContainer_ID)
                .HasColumnName(GetFieldName("AbTestClientRedirectContainer", "BaseContainer"));
 
            #endregion

            #region AbTestClientRedirect mappings
            modelBuilder.Entity<AbTestClientRedirect>()
                .ToTable(GetTableName("AbTestClientRedirect"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<AbTestClientRedirect>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<AbTestClientRedirect>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<AbTestClientRedirect>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<AbTestClientRedirect>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<AbTestClientRedirect>()
                .Property(x => x.VersionNumber)
                .HasColumnName(GetFieldName("AbTestClientRedirect", "VersionNumber"));
            modelBuilder.Entity<AbTestClientRedirect>()
                .Property(x => x.RedirectUrl)
                .HasColumnName(GetFieldName("AbTestClientRedirect", "RedirectUrl"));
            modelBuilder.Entity<AbTestClientRedirect>()
                .HasOne<AbTestClientRedirectContainer>(mp => mp.Container)
                .WithMany(mp => mp.ClientRedirectsInContainer)
                .HasForeignKey(fp => fp.Container_ID);

            modelBuilder.Entity<AbTestClientRedirect>()
                .Property(x => x.Container_ID)
                .HasColumnName(GetFieldName("AbTestClientRedirect", "Container"));
 
            #endregion

            #region BlogPost mappings
            modelBuilder.Entity<BlogPost>()
                .ToTable(GetTableName("BlogPost"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<BlogPost>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<BlogPost>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<BlogPost>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<BlogPost>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<BlogPost>()
                .Property(x => x.Title)
                .HasColumnName(GetFieldName("BlogPost", "Title"));
            modelBuilder.Entity<BlogPost>()
                .Property(x => x.Brief)
                .HasColumnName(GetFieldName("BlogPost", "Brief"));
            modelBuilder.Entity<BlogPost>()
                .Property(x => x.Text)
                .HasColumnName(GetFieldName("BlogPost", "Text"));
            modelBuilder.Entity<BlogPost>()
                .Property(x => x.YoutubeVideoCode)
                .HasColumnName(GetFieldName("BlogPost", "YoutubeVideoCode"));
            modelBuilder.Entity<BlogPost>()
                .HasOne<BlogCategory>(mp => mp.Category)
                .WithMany(mp => mp.PostsInCategory)
                .HasForeignKey(fp => fp.Category_ID);

            modelBuilder.Entity<BlogPost>()
                .Property(x => x.Category_ID)
                .HasColumnName(GetFieldName("BlogPost", "Category"));
			 modelBuilder.Entity<BlogPost2BlogTagForTags>()
                .ToTable(GetLinkTableName("BlogPost", "Tags"));

            modelBuilder.Entity<BlogPost2BlogTagForTags>().Property(e => e.BlogPostItemId).HasColumnName("id");
            modelBuilder.Entity<BlogPost2BlogTagForTags>().Property(e => e.BlogTagLinkedItemId).HasColumnName("linked_id");
            modelBuilder.Entity<BlogPost2BlogTagForTags>().HasKey(ug => new { ug.BlogPostItemId, ug.BlogTagLinkedItemId });

            modelBuilder.Entity<BlogPost2BlogTagForTags>()
                .HasOne(bc => bc.BlogPostItem)
                .WithMany(b => b.Tags)
                .HasForeignKey(bc => bc.BlogPostItemId);

            modelBuilder.Entity<BlogPost2BlogTagForTags>()
                .HasOne<BlogTag>(bc => bc.BlogTagLinkedItem)
                .WithMany(c => c.BackwardForTags)
                .HasForeignKey(bc => bc.BlogTagLinkedItemId);
            modelBuilder.Entity<BlogPost>().Ignore(p => p.ImageUrl);
            modelBuilder.Entity<BlogPost>().Ignore(p => p.ImageUploadPath);
 
            #endregion

            #region BlogCategory mappings
            modelBuilder.Entity<BlogCategory>()
                .ToTable(GetTableName("BlogCategory"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<BlogCategory>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<BlogCategory>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<BlogCategory>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<BlogCategory>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<BlogCategory>()
                .Property(x => x.Title)
                .HasColumnName(GetFieldName("BlogCategory", "Title"));
 
            #endregion

            #region BlogTag mappings
            modelBuilder.Entity<BlogTag>()
                .ToTable(GetTableName("BlogTag"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<BlogTag>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<BlogTag>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<BlogTag>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<BlogTag>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<BlogTag>()
                .Property(x => x.Title)
                .HasColumnName(GetFieldName("BlogTag", "Title"));
 
            #endregion

            #region FaqItem mappings
            modelBuilder.Entity<FaqItem>()
                .ToTable(GetTableName("FaqItem"))
                .Property(x => x.Id)
                .HasColumnName("CONTENT_ITEM_ID");

			 modelBuilder.Entity<FaqItem>()
                .HasKey(x=>x.Id);
           
		    modelBuilder.Entity<FaqItem>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<FaqItem>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<FaqItem>()
                .HasOne<StatusType>(x => x.StatusType)
                .WithMany()
				.IsRequired()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<FaqItem>()
                .Property(x => x.Question)
                .HasColumnName(GetFieldName("FaqItem", "Question"));
            modelBuilder.Entity<FaqItem>()
                .Property(x => x.Answer)
                .HasColumnName(GetFieldName("FaqItem", "Answer"));
 
            #endregion
			AddMappingInfo(modelBuilder.Model);
        }
    }
}
