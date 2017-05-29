using System.Data.SqlClient;
using Xunit;
using Common.Persistent.Dapper;
using Microsoft.Extensions.Caching.Memory;
using Common.PageModel;
using Common.Persistent.Data;

namespace Test.Integration.Persistent.Dupper
{
    public class AbstractItemRepositoryTest
    {
        const string connectionString = "Application Name=QP7.Qa_Beeline_Main;Initial Catalog=qp_beeline_main_cis;Data Source=mscsql01;User ID=publishing;Password=QuantumartHost.SQL;";

        [Fact]
        public void GetAllTest()
        {
            var storageProvider = new QpAbstractItemStorageProvider(new UnitOfWork(connectionString), new AbstractItemActivator());
            var storage = storageProvider.Get();

            Assert.True(true);
        }

    }
   
}
