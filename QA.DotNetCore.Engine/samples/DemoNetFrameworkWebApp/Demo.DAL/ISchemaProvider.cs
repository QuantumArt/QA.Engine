using Quantumart.QP8.CodeGeneration.Services;

namespace Demo.DAL
{
    public interface ISchemaProvider
    {
        ModelReader GetSchema();
        object GetCacheKey();
    }
}