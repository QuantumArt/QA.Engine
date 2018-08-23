using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using Quantumart.QP8.CodeGeneration.Services;
/* place your custom usings here */

namespace QA.DemoSite.DAL
{
    public class MappingConfigurator : MappingConfiguratorBase
    {
        public MappingConfigurator(ContentAccess contentAccess, ISchemaProvider schemaProvider)
            : base(contentAccess, schemaProvider)
        {
        }

        public override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        base.OnModelCreating(modelBuilder);

            #region QPAbstractItem mappings
            modelBuilder.Entity<QPAbstractItem>()
                .ToTable(GetTableName("QPAbstractItem"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QPAbstractItem>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.TitleFormat_ID)
                .HasColumnName(GetFieldName("QPAbstractItem", "TitleFormat_ID"));
            modelBuilder.Entity<QPAbstractItem>()
                .HasOptional<QPAbstractItem>(mp => mp.Parent)
                .WithMany(mp => mp.Children)
                .HasForeignKey(fp => fp.Parent_ID);

            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.Parent_ID)
                .HasColumnName(GetFieldName("QPAbstractItem", "Parent"));
            modelBuilder.Entity<QPAbstractItem>()
                .HasOptional<QPDiscriminator>(mp => mp.Discriminator)
                .WithMany(mp => mp.Items)
                .HasForeignKey(fp => fp.Discriminator_ID);

            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.Discriminator_ID)
                .HasColumnName(GetFieldName("QPAbstractItem", "Discriminator"));
            modelBuilder.Entity<QPAbstractItem>()
                .HasOptional<QPAbstractItem>(mp => mp.VersionOf)
                .WithMany(mp => mp.Versions)
                .HasForeignKey(fp => fp.VersionOf_ID);

            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.VersionOf_ID)
                .HasColumnName(GetFieldName("QPAbstractItem", "VersionOf"));
            modelBuilder.Entity<QPAbstractItem>()
                .HasOptional<QPCulture>(mp => mp.Culture)
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
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<QPDiscriminator>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QPDiscriminator>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QPDiscriminator>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 


            modelBuilder.Entity<QPDiscriminator>().HasMany<QPDiscriminator>(p => p.AllowedItemDefinitions1).WithMany()
                .Map(rp =>
                {
                    rp.MapLeftKey("id");
                    rp.MapRightKey("linked_id");
                    rp.ToTable(GetLinkTableName("QPDiscriminator", "AllowedItemDefinitions1"));
                });

            modelBuilder.Entity<QPDiscriminator>().HasMany<QPDiscriminator>(p => p.BackwardForAllowedItemDefinitions1).WithMany()
                .Map(rp =>
                { 
                    rp.MapLeftKey("linked_id"); // ===
                    rp.MapRightKey("id");
                    rp.ToTable(GetReversedLinkTableName("QPDiscriminator", "AllowedItemDefinitions1"));
                });

            modelBuilder.Entity<QPDiscriminator>().Ignore(p => p.IconUrlUrl);
            modelBuilder.Entity<QPDiscriminator>().Ignore(p => p.IconUrlUploadPath);
 
            #endregion

            #region QPCulture mappings
            modelBuilder.Entity<QPCulture>()
                .ToTable(GetTableName("QPCulture"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<QPCulture>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QPCulture>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QPCulture>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<QPCulture>().Ignore(p => p.IconUrl);
            modelBuilder.Entity<QPCulture>().Ignore(p => p.IconUploadPath);
 
            #endregion

            #region QPItemDefinitionConstraint mappings
            modelBuilder.Entity<QPItemDefinitionConstraint>()
                .ToTable(GetTableName("QPItemDefinitionConstraint"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<QPItemDefinitionConstraint>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QPItemDefinitionConstraint>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QPItemDefinitionConstraint>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<QPItemDefinitionConstraint>()
                .HasOptional<QPDiscriminator>(mp => mp.Target)
                .WithMany(mp => mp.AllowDefinition)
                .HasForeignKey(fp => fp.Target_ID);

            modelBuilder.Entity<QPItemDefinitionConstraint>()
                .Property(x => x.Target_ID)
                .HasColumnName(GetFieldName("QPItemDefinitionConstraint", "Target"));
            modelBuilder.Entity<QPItemDefinitionConstraint>()
                .HasOptional<QPDiscriminator>(mp => mp.Source)
                .WithMany(mp => mp.AllowedItemDefinitions)
                .HasForeignKey(fp => fp.Source_ID);

            modelBuilder.Entity<QPItemDefinitionConstraint>()
                .Property(x => x.Source_ID)
                .HasColumnName(GetFieldName("QPItemDefinitionConstraint", "Source"));
 
            #endregion

            #region BlogPost mappings
            modelBuilder.Entity<BlogPost>()
                .ToTable(GetTableName("BlogPost"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<BlogPost>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<BlogPost>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<BlogPost>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<BlogPost>()
                .HasOptional<BlogCategory>(mp => mp.Category)
                .WithMany(mp => mp.PostsInCategory)
                .HasForeignKey(fp => fp.Category_ID);

            modelBuilder.Entity<BlogPost>()
                .Property(x => x.Category_ID)
                .HasColumnName(GetFieldName("BlogPost", "Category"));

            modelBuilder.Entity<BlogPost>().HasMany<BlogTag>(p => p.Tags)
                .WithMany(r => r.BackwardForTags)
                .Map(rp =>
                {
                    rp.MapLeftKey("id");
                    rp.MapRightKey("linked_id");
                    rp.ToTable(GetLinkTableName("BlogPost", "Tags"));
                });
            modelBuilder.Entity<BlogPost>().Ignore(p => p.ImageUrl);
            modelBuilder.Entity<BlogPost>().Ignore(p => p.ImageUploadPath);
 
            #endregion

            #region BlogCategory mappings
            modelBuilder.Entity<BlogCategory>()
                .ToTable(GetTableName("BlogCategory"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<BlogCategory>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<BlogCategory>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<BlogCategory>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

 
            #endregion

            #region BlogTag mappings
            modelBuilder.Entity<BlogTag>()
                .ToTable(GetTableName("BlogTag"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<BlogTag>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<BlogTag>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<BlogTag>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

 
            #endregion

            #region FaqItem mappings
            modelBuilder.Entity<FaqItem>()
                .ToTable(GetTableName("FaqItem"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<FaqItem>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<FaqItem>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<FaqItem>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

 
            #endregion
        }
    }
}
