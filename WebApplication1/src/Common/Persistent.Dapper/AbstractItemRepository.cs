using Common.PageModel;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using System;
using Common.Persistent.Data;

namespace Common.Persistent.Dapper
{
    internal class AbstractItemRepository : IAbstractItemRepository
    {

        private readonly IDbConnection _connection;

        IMemoryCache _memoryCache;

        public AbstractItemRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        private const string CmdGetAbstractItemContentId = @"
SELECT CONTENT_ID AS ContentId
FROM content
WHERE NET_CONTENT_NAME = 'QPAbstractItem'";

        private const string CmdGetAbstractItem = @"
SELECT
    content_item_id AS Id,
    Name as Alias,
    Title,
    Visible,
    Discriminator,
    Parent AS ParentId
FROM {0}
WHERE
    IsPage = 1 AND
    VISIBLE = 1 AND
    ARCHIVE = 0";

        string GetAbstractItemTable()
        {
            var result =  _connection.Query(CmdGetAbstractItemContentId).First();
            return $"content_{result.ContentId}";
        }

        //AbstractItem ConvertAbstractItem(AbstractItemPersistentData state)
        //{
        //    if (!state.Children.Any())
        //    {
        //        return new TextPage(state.Id, state.Alias, state.Title);
        //    }

        //    AbstractItem[] children = new AbstractItem[state.Children.Count()];
        //    int i = 0;
        //    foreach (var childState in state.Children)
        //    {
        //        children[i] = ConvertAbstractItem(childState);
        //        i++;
        //    }

        //    return new TextPage(state.Id, state.Alias, state.Title, children);
        //}

        public IEnumerable<AbstractItemPersistentData> GetPlainAllAbstractItems()
        {
            var itemsTable = GetAbstractItemTable();
            return _connection.Query<AbstractItemPersistentData>(string.Format(CmdGetAbstractItem, itemsTable));
        }

    }
}
