using System;
using System.Collections;

namespace QA.DemoSite.Postgre.DAL
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
