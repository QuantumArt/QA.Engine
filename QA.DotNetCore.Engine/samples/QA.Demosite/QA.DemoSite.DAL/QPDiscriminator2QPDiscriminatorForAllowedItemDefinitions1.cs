// Code generated by a template
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace QA.DemoSite.Mssql.DAL
{
    public partial class QPDiscriminator2QPDiscriminatorForAllowedItemDefinitions1: IQPLink
    {
		public QPDiscriminator QPDiscriminatorItem { get; set; }		
		public QPDiscriminator QPDiscriminatorLinkedItem { get; set; }

		public int QPDiscriminatorItemId { get; set; }	
		public int QPDiscriminatorLinkedItemId { get; set; }

		public int LinkId
		{
			get { return 88; }
		}

		public int Id 
		{ 
			get { return QPDiscriminatorItemId; } 
			set { QPDiscriminatorItemId = value; } 
		}
        public int LinkedItemId 
		{ 
			get { return QPDiscriminatorLinkedItemId; }
			set { QPDiscriminatorLinkedItemId = value; }
		}
		public IQPArticle Item { get { return QPDiscriminatorItem; } }		
		public IQPArticle LinkedItem { get { return QPDiscriminatorLinkedItem; } }
	}

	public partial class QPDiscriminator2QPDiscriminatorForBackwardForAllowedItemDefinitions1: IQPLink
    {
		public QPDiscriminator QPDiscriminatorLinkedItem { get; set; }		
		public QPDiscriminator QPDiscriminatorItem { get; set; }

		public int QPDiscriminatorLinkedItemId { get; set; }	
		public int QPDiscriminatorItemId { get; set; }

		public int LinkId
		{
			get { return 88; }
		}

		public int Id 
		{ 
			get { return QPDiscriminatorItemId; } 
			set { QPDiscriminatorItemId = value; } 
		}
		public int LinkedItemId 
		{ 
			get { return QPDiscriminatorLinkedItemId; } 
			set { QPDiscriminatorLinkedItemId = value; } 
		}
		public IQPArticle Item { get { return QPDiscriminatorItem; } }		
		public IQPArticle LinkedItem { get { return QPDiscriminatorLinkedItem; } }

	}

}
	
	
	
	
	
	
	
	
	
