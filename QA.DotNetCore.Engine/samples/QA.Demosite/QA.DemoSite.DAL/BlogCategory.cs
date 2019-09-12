// Code generated by a template
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace QA.DemoSite.Mssql.DAL
{
    public partial class BlogCategory: IQPArticle
    {
        public BlogCategory()
        {
		    PostsInCategory = new HashSet<BlogPost>();
        }

        public virtual Int32 Id { get; set; }
        public virtual Int32 StatusTypeId { get; set; }
        public virtual bool Visible { get; set; }
        public virtual bool Archive { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual Int32 LastModifiedBy { get; set; }
        public virtual StatusType StatusType { get; set; }

		private String _internalTitle;
		public virtual String Title 
		{ 
			get { return _internalTitle; }
			set { _internalTitle = QpDataContext.Current.ReplacePlaceholders(value);}
		}
        public virtual Int32? SortOrder { get; set; }
		/// <summary>
		/// Auto-generated backing property for field (id: 68569)/Category PostsInCategory
		/// </summary>
		public  ICollection<BlogPost> PostsInCategory { get; set; }
	}
}
	
