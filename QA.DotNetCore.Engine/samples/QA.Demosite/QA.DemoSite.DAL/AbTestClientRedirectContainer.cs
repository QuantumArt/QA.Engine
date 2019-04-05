// Code generated by a template
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Quantumart.QP8.EntityFrameworkCore;
namespace QA.DemoSite.DAL
{
    public partial class AbTestClientRedirectContainer: IQPArticle
    {
        public AbTestClientRedirectContainer()
        {
		    ClientRedirectsInContainer = new HashSet<AbTestClientRedirect>();
        }

        public virtual Int32 Id { get; set; }
        public virtual Int32 StatusTypeId { get; set; }
        public virtual bool Visible { get; set; }
        public virtual bool Archive { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual Int32 LastModifiedBy { get; set; }
        public virtual StatusType StatusType { get; set; }

		/// <summary>
		/// 
		/// </summary>			
		public virtual AbTestBaseContainer BaseContainer { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public virtual Int32? BaseContainer_ID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public  ICollection<AbTestClientRedirect> ClientRedirectsInContainer { get; set; }
	}
}
	
