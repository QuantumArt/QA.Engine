// Code generated by a template
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
namespace Demo.DAL
{
    public partial class QpBlog: IQPArticle
    {
        public QpBlog()
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

        public virtual String PostTitle { get; set; }
        public virtual String PostDescription { get; set; }
        public virtual String PostText { get; set; }
        public virtual String PostImgDescription { get; set; }
        public virtual String PostImage { get; set; }
		#region Generated Content properties
        // public string PostImageUrl { get; set; }
        // public string PostImageUploadPath { get; set; }
		#endregion
	}
}
