using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Quantumart.QP8.CoreCodeGeneration.Services;

namespace QA.DemoSite.Mssql.DAL
{
    public interface IMappingConfigurator
    {
        MappingInfo GetMappingInfo();
        void OnModelCreating(ModelBuilder modelBuilder);
		ModelReader GetSchema();
    }

    public class MappingInfo
    {
        public IModel DbCompiledModel { get; private set; }
        public ModelReader Schema { get; private set; }

        public MappingInfo(IModel dbCompiledModel, ModelReader schema)
        {
            DbCompiledModel = dbCompiledModel;
            Schema = schema;
        }
    }
    public enum ContentAccess
    {
        /// <summary>
        /// Published articles
        /// </summary>
        Live = 0,
        /// <summary>
        /// Splitted versions of articles
        /// </summary>
        Stage = 1,
        /// <summary>
        /// Splitted versions of articles including invisible and archived (overrides useDefaultFiltration content setting)
        /// </summary>
        StageNoDefaultFiltration = 2
    }
}