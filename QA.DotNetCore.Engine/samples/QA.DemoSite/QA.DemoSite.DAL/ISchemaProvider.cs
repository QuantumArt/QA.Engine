using Quantumart.QP8.CodeGeneration.Services;

namespace QA.DemoSite.DAL
{
    public interface ISchemaProvider
    {
        ModelReader GetSchema();
        object GetCacheKey();
    }
}