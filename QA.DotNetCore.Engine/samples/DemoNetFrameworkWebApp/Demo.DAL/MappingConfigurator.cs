using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using Quantumart.QP8.CodeGeneration.Services;


namespace Demo.DAL
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
            modelBuilder.Entity<QPAbstractItem>()
                .HasOptional<ItemTitleFormat>(mp => mp.TitleFormat)
                .WithMany(mp => mp.Item)
                .HasForeignKey(fp => fp.TitleFormat_ID);

            modelBuilder.Entity<QPAbstractItem>()
                .Property(x => x.TitleFormat_ID)
                .HasColumnName(GetFieldName("QPAbstractItem", "TitleFormat"));

            modelBuilder.Entity<QPAbstractItem>().HasMany<QPRegion>(p => p.Regions)
                .WithMany(r => r.BackwardForRegions)
                .Map(rp =>
                {
                    rp.MapLeftKey("id");
                    rp.MapRightKey("linked_id");
                    rp.ToTable(GetLinkTableName("QPAbstractItem", "Regions"));
                });
 
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

            #region MarketingRegion mappings
            modelBuilder.Entity<MarketingRegion>()
                .ToTable(GetTableName("MarketingRegion"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<MarketingRegion>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<MarketingRegion>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<MarketingRegion>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<MarketingRegion>()
                .HasOptional<MarketingRegion>(mp => mp.Parent)
                .WithMany(mp => mp.Children)
                .HasForeignKey(fp => fp.Parent_ID);

            modelBuilder.Entity<MarketingRegion>()
                .Property(x => x.Parent_ID)
                .HasColumnName(GetFieldName("MarketingRegion", "Parent"));
 
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

            #region ItemTitleFormat mappings
            modelBuilder.Entity<ItemTitleFormat>()
                .ToTable(GetTableName("ItemTitleFormat"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<ItemTitleFormat>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<ItemTitleFormat>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<ItemTitleFormat>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

 
            #endregion

            #region QPRegion mappings
            modelBuilder.Entity<QPRegion>()
                .ToTable(GetTableName("QPRegion"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<QPRegion>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QPRegion>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QPRegion>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

 
            #endregion

            #region TrailedAbstractItem mappings
            modelBuilder.Entity<TrailedAbstractItem>()
                .ToTable(GetTableName("TrailedAbstractItem"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<TrailedAbstractItem>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<TrailedAbstractItem>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<TrailedAbstractItem>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

 
            #endregion

            #region SiteSetting mappings
            modelBuilder.Entity<SiteSetting>()
                .ToTable(GetTableName("SiteSetting"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<SiteSetting>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<SiteSetting>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<SiteSetting>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

 
            #endregion

            #region QpRegionTag mappings
            modelBuilder.Entity<QpRegionTag>()
                .ToTable(GetTableName("QpRegionTag"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<QpRegionTag>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QpRegionTag>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QpRegionTag>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

 
            #endregion

            #region QpRegionTagValue mappings
            modelBuilder.Entity<QpRegionTagValue>()
                .ToTable(GetTableName("QpRegionTagValue"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<QpRegionTagValue>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QpRegionTagValue>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QpRegionTagValue>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<QpRegionTagValue>()
                .HasOptional<QpRegionTag>(mp => mp.Parent)
                .WithMany(mp => mp.QpRegionTagValues)
                .HasForeignKey(fp => fp.Parent_ID);

            modelBuilder.Entity<QpRegionTagValue>()
                .Property(x => x.Parent_ID)
                .HasColumnName(GetFieldName("QpRegionTagValue", "Parent"));

            modelBuilder.Entity<QpRegionTagValue>().HasMany<MarketingRegion>(p => p.Regions).WithMany()
                .Map(rp =>
                {
                    rp.MapLeftKey("id");
                    rp.MapRightKey("linked_id");
                    rp.ToTable(GetLinkTableName("QpRegionTagValue", "Regions"));
                });

            modelBuilder.Entity<MarketingRegion>().HasMany<QpRegionTagValue>(p => p.BackwardForRegions).WithMany()
                .Map(rp =>
                { 
                    rp.MapLeftKey("id"); // !+
                    rp.MapRightKey("linked_id");
                    rp.ToTable(GetReversedLinkTableName("QpRegionTagValue", "Regions"));
                });

 
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

            #region News mappings
            modelBuilder.Entity<News>()
                .ToTable(GetTableName("News"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<News>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<News>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<News>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<News>()
                .Property(x => x.PublishDate)
                .HasColumnName(GetFieldName("News", "PublishDate"));
            modelBuilder.Entity<News>()
                .Property(x => x.Alias)
                .HasColumnName(GetFieldName("News", "Alias"));
            modelBuilder.Entity<News>()
                .HasOptional<QpSMI>(mp => mp.PublishSMI)
                .WithMany(mp => mp.News)
                .HasForeignKey(fp => fp.PublishSMI_ID);

            modelBuilder.Entity<News>()
                .Property(x => x.PublishSMI_ID)
                .HasColumnName(GetFieldName("News", "PublishSMI"));
            modelBuilder.Entity<News>()
                .HasOptional<ItemTitleFormat>(mp => mp.TitleFormat)
                .WithMany(mp => mp.News)
                .HasForeignKey(fp => fp.TitleFormat_ID);

            modelBuilder.Entity<News>()
                .Property(x => x.TitleFormat_ID)
                .HasColumnName(GetFieldName("News", "TitleFormat"));

            modelBuilder.Entity<News>().HasMany<NewsGroup>(p => p.Groups)
                .WithMany(r => r.BackwardForGroups)
                .Map(rp =>
                {
                    rp.MapLeftKey("id");
                    rp.MapRightKey("linked_id");
                    rp.ToTable(GetLinkTableName("News", "Groups"));
                });

            modelBuilder.Entity<News>().HasMany<MarketingRegion>(p => p.Regions).WithMany()
                .Map(rp =>
                {
                    rp.MapLeftKey("id");
                    rp.MapRightKey("linked_id");
                    rp.ToTable(GetLinkTableName("News", "Regions"));
                });

            modelBuilder.Entity<MarketingRegion>().HasMany<News>(p => p.BackwardForRegions1).WithMany()
                .Map(rp =>
                { 
                    rp.MapLeftKey("id"); // !+
                    rp.MapRightKey("linked_id");
                    rp.ToTable(GetReversedLinkTableName("News", "Regions"));
                });


            modelBuilder.Entity<News>().HasMany<QpNewsRubric>(p => p.Rubrics).WithMany()
                .Map(rp =>
                {
                    rp.MapLeftKey("id");
                    rp.MapRightKey("linked_id");
                    rp.ToTable(GetLinkTableName("News", "Rubrics"));
                });

            modelBuilder.Entity<QpNewsRubric>().HasMany<News>(p => p.BackwardForRubrics).WithMany()
                .Map(rp =>
                { 
                    rp.MapLeftKey("id"); // !+
                    rp.MapRightKey("linked_id");
                    rp.ToTable(GetReversedLinkTableName("News", "Rubrics"));
                });

            modelBuilder.Entity<News>().Ignore(p => p.MainImageUrl);
            modelBuilder.Entity<News>().Ignore(p => p.SmallImageUrl);
            modelBuilder.Entity<News>().Ignore(p => p.MainImageUploadPath);
            modelBuilder.Entity<News>().Ignore(p => p.SmallImageUploadPath);
 
            #endregion

            #region NewsGroup mappings
            modelBuilder.Entity<NewsGroup>()
                .ToTable(GetTableName("NewsGroup"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<NewsGroup>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<NewsGroup>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<NewsGroup>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

 
            #endregion

            #region QpSMI mappings
            modelBuilder.Entity<QpSMI>()
                .ToTable(GetTableName("QpSMI"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<QpSMI>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QpSMI>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QpSMI>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

 
            #endregion

            #region QpNewsRubric mappings
            modelBuilder.Entity<QpNewsRubric>()
                .ToTable(GetTableName("QpNewsRubric"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<QpNewsRubric>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QpNewsRubric>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QpNewsRubric>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

 
            #endregion

            #region PopularServicesValue mappings
            modelBuilder.Entity<PopularServicesValue>()
                .ToTable(GetTableName("PopularServicesValue"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<PopularServicesValue>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<PopularServicesValue>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<PopularServicesValue>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

 
            #endregion

            #region MainSectionsValue mappings
            modelBuilder.Entity<MainSectionsValue>()
                .ToTable(GetTableName("MainSectionsValue"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<MainSectionsValue>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<MainSectionsValue>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<MainSectionsValue>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

 
            #endregion

            #region QpAction mappings
            modelBuilder.Entity<QpAction>()
                .ToTable(GetTableName("QpAction"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<QpAction>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QpAction>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QpAction>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 


            modelBuilder.Entity<QpAction>().HasMany<MarketingRegion>(p => p.Regions).WithMany()
                .Map(rp =>
                {
                    rp.MapLeftKey("id");
                    rp.MapRightKey("linked_id");
                    rp.ToTable(GetLinkTableName("QpAction", "Regions"));
                });

            modelBuilder.Entity<MarketingRegion>().HasMany<QpAction>(p => p.BackwardForRegions2).WithMany()
                .Map(rp =>
                { 
                    rp.MapLeftKey("id"); // !+
                    rp.MapRightKey("linked_id");
                    rp.ToTable(GetReversedLinkTableName("QpAction", "Regions"));
                });

 
            #endregion

            #region QpBlog mappings
            modelBuilder.Entity<QpBlog>()
                .ToTable(GetTableName("QpBlog"))
                .Property(x => x.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
                .HasColumnName("CONTENT_ITEM_ID");
           
		    modelBuilder.Entity<QpBlog>()
                .Property(x => x.LastModifiedBy)
                .HasColumnName("LAST_MODIFIED_BY");
            
            modelBuilder.Entity<QpBlog>()
                .Property(x => x.StatusTypeId)
                .HasColumnName("STATUS_TYPE_ID");

			modelBuilder.Entity<QpBlog>()
                .HasRequired<StatusType>(x => x.StatusType)
                .WithMany()
                .HasForeignKey(x => x.StatusTypeId); 

            modelBuilder.Entity<QpBlog>().Ignore(p => p.PostImageUrl);
            modelBuilder.Entity<QpBlog>().Ignore(p => p.PostImageUploadPath);
 
            #endregion
        }
    }
}
