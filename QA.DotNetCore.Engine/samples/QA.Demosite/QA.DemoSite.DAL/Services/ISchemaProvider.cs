
using Quantumart.QP8.CoreCodeGeneration.Services;

namespace Quantumart.QP8.EntityFrameworkCore
{
    public interface ISchemaProvider
    {
        ModelReader GetSchema();
        object GetCacheKey();
    }
}