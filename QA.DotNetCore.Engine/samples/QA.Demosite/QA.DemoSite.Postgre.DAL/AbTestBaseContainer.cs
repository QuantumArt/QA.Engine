// Code generated by a template
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace QA.DemoSite.Postgre.DAL
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

		private String _internalDescription;
		public virtual String Description 
		{ 
			get { return _internalDescription; }
			set { _internalDescription = PostgreQpDataContext.Current.ReplacePlaceholders(value);}
		}
		private String _internalAllowedUrlPatterns;
		public virtual String AllowedUrlPatterns 
		{ 
			get { return _internalAllowedUrlPatterns; }
			set { _internalAllowedUrlPatterns = PostgreQpDataContext.Current.ReplacePlaceholders(value);}
		}
		private String _internalDeniedUrlPatterns;
		public virtual String DeniedUrlPatterns 
		{ 
			get { return _internalDeniedUrlPatterns; }
			set { _internalDeniedUrlPatterns = PostgreQpDataContext.Current.ReplacePlaceholders(value);}
		}
		private String _internalDomain;
		public virtual String Domain 
		{ 
			get { return _internalDomain; }
			set { _internalDomain = PostgreQpDataContext.Current.ReplacePlaceholders(value);}
		}
		private String _internalPrecondition;
		public virtual String Precondition 
		{ 
			get { return _internalPrecondition; }
			set { _internalPrecondition = PostgreQpDataContext.Current.ReplacePlaceholders(value);}
		}
		private String _internalArguments;
		public virtual String Arguments 
		{ 
			get { return _internalArguments; }
			set { _internalArguments = PostgreQpDataContext.Current.ReplacePlaceholders(value);}
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
	
