
using Quantumart.QP8.CoreCodeGeneration.Services;

namespace QA.DemoSite.DAL
{
    public interface ISchemaProvider
    {
        ModelReader GetSchema();
        object GetCacheKey();
    }
}