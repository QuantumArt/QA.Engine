// Code generated by a template
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Quantumart.QP8.EntityFrameworkCore;
namespace QA.DemoSite.DAL
{
    public partial class AbTestBaseContainer: IQPArticle
    {
        public AbTestBaseContainer()
        {
		    ScriptContainers = new HashSet<AbTestScriptContainer>();
		    ClientRedirectContainers = new HashSet<AbTestClientRedirectContainer>();
        }

        public virtual Int32 Id { get; set; }
        public virtual Int32 StatusTypeId { get; set; }
        public virtual bool Visible { get; set; }
        public virtual bool Archive { get; set; }
        public virtual DateTime Created { get; set; }
        public virtual DateTime Modified { get; set; }
        public virtual Int32 LastModifiedBy { get; set; }
        public virtual StatusType StatusType { get; set; }

		private String _Description;
		public virtual String Description 
		{ 
			get { return _Description; }
			set { _Description = QpDataContext.Current.ReplacePlaceholders(value);}
		}
		private String _AllowedUrlPatterns;
		public virtual String AllowedUrlPatterns 
		{ 
			get { return _AllowedUrlPatterns; }
			set { _AllowedUrlPatterns = QpDataContext.Current.ReplacePlaceholders(value);}
		}
		private String _DeniedUrlPatterns;
		public virtual String DeniedUrlPatterns 
		{ 
			get { return _DeniedUrlPatterns; }
			set { _DeniedUrlPatterns = QpDataContext.Current.ReplacePlaceholders(value);}
		}
		private String _Domain;
		public virtual String Domain 
		{ 
			get { return _Domain; }
			set { _Domain = QpDataContext.Current.ReplacePlaceholders(value);}
		}
		private String _Precondition;
		public virtual String Precondition 
		{ 
			get { return _Precondition; }
			set { _Precondition = QpDataContext.Current.ReplacePlaceholders(value);}
		}
		private String _Arguments;
		public virtual String Arguments 
		{ 
			get { return _Arguments; }
			set { _Arguments = QpDataContext.Current.ReplacePlaceholders(value);}
		}
        public virtual Int32? Type { get; set; }
		/// <summary>
		/// 
		/// </summary>			
		public virtual AbTest ParentTest { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public virtual Int32? ParentTest_ID { get; set; }
		/// <summary>
		/// Auto-generated backing property for field (id: 48224)/BaseContainer ScriptContainers
		/// </summary>
		public  ICollection<AbTestScriptContainer> ScriptContainers { get; set; }
		/// <summary>
		/// Auto-generated backing property for field (id: 48230)/BaseContainer ClientRedirectContainers
		/// </summary>
		public  ICollection<AbTestClientRedirectContainer> ClientRedirectContainers { get; set; }
	}
}
	
