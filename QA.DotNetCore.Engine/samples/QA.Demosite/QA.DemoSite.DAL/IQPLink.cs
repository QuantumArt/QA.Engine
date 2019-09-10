using System;
using System.Collections;

namespace QA.DemoSite.Mssql.DAL
{
    public interface IQPLink
    {
        int Id { get; set; }
        int LinkedItemId { get; set; }
        int LinkId { get; }

        IQPArticle Item { get; }
        IQPArticle LinkedItem { get; }
    }

}
