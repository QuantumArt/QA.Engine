using System.Data.SqlClient;
using Xunit;
using Common.Persistent.Dupper;

namespace Test.Integration.Persistent.Dupper
{
    public class AbstractItemRepositoryTest
    {
        const string connectionString = "Application Name=QP7.Qa_Beeline_Main;Initial Catalog=qp_beeline_main_cis;Data Source=mscsql01;User ID=publishing;Password=QuantumartHost.SQL;";

        [Fact]
        public void GetAllTest()
        {
            //qp_database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var repo = new AbstractItemRepository(connection);
                var items = repo.GetAll();
            }

            Assert.True(true);
        }

    }
   
}
