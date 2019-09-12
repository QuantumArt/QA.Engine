
using Quantumart.QP8.CoreCodeGeneration.Services;

namespace QA.DemoSite.Mssql.DAL
{
    public class FileSchemaProvider : ISchemaProvider
    {
        private readonly string _path;

        public FileSchemaProvider(string path)
        {
            _path = path;
        }

        #region ISchemaProvider implementation
        public ModelReader GetSchema()
        {
            return new ModelReader(_path, _ => { });
        }

        public object GetCacheKey()
        {
            return _path;
        }
        #endregion
    }
}
