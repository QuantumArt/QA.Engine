using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;	


namespace QA.DemoSite.DAL
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
        public virtual DbSet<QPCulture> QPCultures { get; set; }
        public virtual DbSet<QPItemDefinitionConstraint> QPItemDefinitionConstraints { get; set; }
        public virtual DbSet<BlogPost> BlogPosts { get; set; }
        public virtual DbSet<BlogCategory> BlogCategories { get; set; }
        public virtual DbSet<BlogTag> BlogTags { get; set; }
        public virtual DbSet<FaqItem> FaqItems { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var schemaProvider = new StaticSchemaProvider();
            var mapping = new MappingConfigurator(DefaultContentAccess, schemaProvider);
            mapping.OnModelCreating(modelBuilder);
        }
	}
}
