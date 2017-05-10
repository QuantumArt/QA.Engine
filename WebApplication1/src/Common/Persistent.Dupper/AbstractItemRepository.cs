using Common.PageModel;
using Common.Persistent.State;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Common.Persistent.Dupper
{
    public class AbstractItemRepository : IAbstractItemRepository
    {

        private readonly IDbConnection _connection;

        IMemoryCache _memoryCache;

        public AbstractItemRepository(IDbConnection connection, IMemoryCache memoryCache)
        {
            _connection = connection;
            _memoryCache = memoryCache;
        }

        //const string _connectionString = "Application Name=QP7.Qa_Beeline_Main;Initial Catalog=qp_beeline_main_cis;Data Source=mscsql01;User ID=publishing;Password=QuantumartHost.SQL;";


        private const string CmdGetAbstractItemContentId = @"
SELECT CONTENT_ID AS ContentId
FROM content
WHERE NET_CONTENT_NAME = 'QPAbstractItem'";

        private const string CmdGetAbstractItem = @"
SELECT
    content_item_id AS Id,
    Title,
    Visible,
    Parent AS ParentId
FROM {0}
WHERE
    IsPage = 1 AND
    VISIBLE = 1 AND
    ARCHIVE = 0";

        string GetAbstarctItemTable()
        {
            var result =  _connection.Query(CmdGetAbstractItemContentId).First();
            return $"content_{result.ContentId}";
        }

        AbstractItem StateToItem(AbstractItemState state)
        {

            if (!state.Children.Any())
            {
                return new TextPage(state.Id, "someAlias", state.Title);
            }

            AbstractItem[] children = new AbstractItem[state.Children.Count()];
            int i = 0;
            foreach (var childState in state.Children)
            {
                children[i] = StateToItem(childState);
                i++;
            }

            return new TextPage(state.Id, "someAlias", state.Title, children);
        }


        public IEnumerable<AbstractItem> GetAll()
        {
            const string key = "AbstractItemRepository_GetAll";
            return _memoryCache.GetOrCreate(key, (cacheEntry) =>
            {
                cacheEntry.SetAbsoluteExpiration(TimeSpan.FromSeconds(1));
                var itemsTable = GetAbstarctItemTable();
                var states = _connection.Query<AbstractItemState>(string.Format(CmdGetAbstractItem, itemsTable));

                foreach (var item in states)
                {
                    var children = states.Where(s => s.ParentId == item.Id);
                    item.Children = children;
                }

                return states.Where(s => !s.ParentId.HasValue).Select(s => StateToItem(s)).ToArray();
            });
        }
        
    }
}
