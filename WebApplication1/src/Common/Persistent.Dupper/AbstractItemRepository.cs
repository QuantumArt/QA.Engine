using Common.Persistent.State;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Common.Persistent.Dupper
{
    public class AbstractItemRepository : IAbstractItemRepository
    {

        private readonly IDbConnection _connection;
        public AbstractItemRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        //const string _connectionString = "Application Name=QP7.Qa_Beeline_Main;Initial Catalog=qp_beeline_main_cis;Data Source=mscsql01;User ID=publishing;Password=QuantumartHost.SQL;";


        private const string CmdGetAbstractItemContentId = @"
SELECT CONTENT_ID AS ContentId
FROM content
WHERE NET_CONTENT_NAME = 'QPAbstractItem'";

        private const string CmdGetAbstractItem = @"
SELECT Title FROM {0}
WHERE
Parent is null AND 
IsPage = 1 AND
VISIBLE = 1 AND
ARCHIVE = 0";

        string GetAbstarctItemTable()
        {
            var result =  _connection.Query(CmdGetAbstractItemContentId).First();
            return $"content_{result.ContentId}";
        }


        public IEnumerable<AbstractItemState> GetAll()
        {
            var itemsTable = GetAbstarctItemTable();
            return _connection.Query<AbstractItemState>(string.Format(CmdGetAbstractItem, itemsTable));
        }
        
    }
}
