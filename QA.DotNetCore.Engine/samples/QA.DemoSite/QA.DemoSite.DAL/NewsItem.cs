// Code generated by a template
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
namespace QA.DemoSite.DAL
{
    public partial class NewsItem: IQPArticle
    {
        public NewsItem()
        {
        }

        public virtual Int32 Id { get; set; }
        public virtual Int32 StatusTypeId { get; set; }
        public virtual bool Visible { get; set; }
        public virtual bool Archive { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual Int32 LastModifiedBy { get; set; }
        public virtual StatusType StatusType { get; set; }

        public virtual String Title { get; set; }
        public virtual DateTime? Date { get; set; }
        public virtual String Text { get; set; }
		/// <summary>
		/// 
		/// </summary>			
		public virtual NewsCategory Category { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public virtual Int32? Category_ID { get; set; }
	}
}
	
