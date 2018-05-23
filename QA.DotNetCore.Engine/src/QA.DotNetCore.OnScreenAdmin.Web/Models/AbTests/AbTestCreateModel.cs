using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;

namespace QA.DotNetCore.OnScreenAdmin.Web.Models.AbTests
{
    public class AbTestCreateModel
    {
        public int SiteId { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public decimal[] Percentage { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public AbTestContainerCreateModel[] Containers { get; set; }
    }

    public class AbTestContainerCreateModel
    {
        public AbTestContainerType Type { get; set; }
        public string Description { get; set; }
        public string Domain { get; set; }
        public string AllowedUrlPatterns { get; set; }
        public string DeniedUrlPatterns { get; set; }
        public string Precondition { get; set; }
        public string Arguments { get; set; }
        public AbTestContainerVariantModel[] Variants { get; set; }
    }

    public class AbTestContainerVariantModel
    {
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
