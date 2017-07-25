// Code generated by a template
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
namespace Demo.DAL
{
    public partial class QPRegion: IQPArticle
    {
        public QPRegion()
        {
		    BackwardForRegions = new HashSet<QPAbstractItem>();
        }

        public virtual Int32 Id { get; set; }
        public virtual Int32 StatusTypeId { get; set; }
        public virtual bool Visible { get; set; }
        public virtual bool Archive { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual Int32 LastModifiedBy { get; set; }
        public virtual StatusType StatusType { get; set; }

        public virtual Int32? ParentId { get; set; }
        public virtual String Title { get; set; }
        public virtual String Alias { get; set; }
		/// <summary>
		/// Auto-generated backing property for 27529/Regions
		/// </summary>
		public  ICollection<QPAbstractItem> BackwardForRegions { get; set; }
		#region Generated Content properties
        // public Int32 ParentIdExact { get { return this.ParentId == null ? default(Int32) : this.ParentId.Value; } }
		#endregion
	}
}
	