// Code generated by a template
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
namespace Demo.DAL
{
    public partial class QPCulture: IQPArticle
    {
        public QPCulture()
        {
		    AbstractItems = new HashSet<QPAbstractItem>();
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
        public virtual String Name { get; set; }
        public virtual String Icon { get; set; }
		/// <summary>
		/// Auto-generated backing property for field (id: 27521)/Culture AbstractItems
		/// </summary>
		public  ICollection<QPAbstractItem> AbstractItems { get; set; }
		#region Generated Content properties
        // public string IconUrl { get; set; }
        // public string IconUploadPath { get; set; }
		#endregion
	}
}
	