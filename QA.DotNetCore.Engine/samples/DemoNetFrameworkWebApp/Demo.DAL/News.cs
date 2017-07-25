// Code generated by a template
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
namespace Demo.DAL
{
    public partial class News: IQPArticle
    {
        public News()
        {
		    Groups = new HashSet<NewsGroup>();
		    Regions = new HashSet<MarketingRegion>();
		    Rubrics = new HashSet<QpNewsRubric>();
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
        public virtual String TitleForDetails { get; set; }
        public virtual String TransliterationTitle { get; set; }
        public virtual DateTime? PublishDate { get; set; }
        public virtual String Anounce { get; set; }
        public virtual String Text { get; set; }
        public virtual Int32? Order { get; set; }
        public virtual String MainImage { get; set; }
        public virtual String Keywords { get; set; }
        public virtual String MetaDescription { get; set; }
        public virtual String Alias { get; set; }
        public virtual String SmallImage { get; set; }
		/// <summary>
		/// 
		/// </summary>			
		public virtual QpSMI PublishSMI { get; set; }
		/// <summary>
		/// 
		/// </summary>			
		public virtual ItemTitleFormat TitleFormat { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public virtual Int32? PublishSMI_ID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public virtual Int32? TitleFormat_ID { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public  ICollection<NewsGroup> Groups { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public  ICollection<MarketingRegion> Regions { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public  ICollection<QpNewsRubric> Rubrics { get; set; }
		#region Generated Content properties
        // public string MainImageUrl { get; set; }
        // public string SmallImageUrl { get; set; }
        // public string MainImageUploadPath { get; set; }
        // public string SmallImageUploadPath { get; set; }
        // public Int32 OrderExact { get { return this.Order == null ? default(Int32) : this.Order.Value; } }
		#endregion
	}
}
	