using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;	


namespace Demo.DAL
{
    public partial class QpDataContext : DbContext
    {
        public static ContentAccess DefaultContentAccess = ContentAccess.Live;

        partial void OnContextCreated();

        static QpDataContext()
        {
            Database.SetInitializer<QpDataContext>(new NullDatabaseInitializer<QpDataContext>());
        }

        public QpDataContext()
            : base("name=QpDataContext")
        {
            MappingResolver = GetDefaultMappingResolver();
            this.Configuration.LazyLoadingEnabled = true;
            this.Configuration.ProxyCreationEnabled = false;

            OnContextCreated();
        }

        public virtual DbSet<StatusType> StatusTypes { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserGroup> UserGroups { get; set; }

        public virtual DbSet<QPAbstractItem> QPAbstractItems { get; set; }
        public virtual DbSet<QPDiscriminator> QPDiscriminators { get; set; }
        public virtual DbSet<MarketingRegion> MarketingRegions { get; set; }
        public virtual DbSet<QPCulture> QPCultures { get; set; }
        public virtual DbSet<ItemTitleFormat> ItemTitleFormats { get; set; }
        public virtual DbSet<QPRegion> QPRegions { get; set; }
        public virtual DbSet<TrailedAbstractItem> TrailedAbstractItems { get; set; }
        public virtual DbSet<SiteSetting> SiteSettings { get; set; }
        public virtual DbSet<QpRegionTag> QpRegionTags { get; set; }
        public virtual DbSet<QpRegionTagValue> QpRegionTagValues { get; set; }
        public virtual DbSet<QPItemDefinitionConstraint> QPItemDefinitionConstraints { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<NewsGroup> NewsGroups { get; set; }
        public virtual DbSet<QpSMI> QpSMI { get; set; }
        public virtual DbSet<QpNewsRubric> QpNewsRubrics { get; set; }
        public virtual DbSet<PopularServicesValue> PopularServicesValues { get; set; }
        public virtual DbSet<MainSectionsValue> MainSectionsValues { get; set; }
        public virtual DbSet<QpAction> QpActions { get; set; }
        public virtual DbSet<QpBlog> QpBlogs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var schemaProvider = new StaticSchemaProvider();
            var mapping = new MappingConfigurator(DefaultContentAccess, schemaProvider);
            mapping.OnModelCreating(modelBuilder);
        }
	}
}
