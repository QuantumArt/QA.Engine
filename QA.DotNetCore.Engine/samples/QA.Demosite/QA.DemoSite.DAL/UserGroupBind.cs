using System.Collections.Generic;

namespace QA.DemoSite.DAL
{
    public partial class UserGroupBind
    {
        public UserGroupBind()
        {

        }
        public virtual int UserId { get; set; }

        public virtual int GroupId { get; set; }

        public virtual User User { get; set; }
        public virtual UserGroup UserGroup { get; set; }
    }
}
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
	
