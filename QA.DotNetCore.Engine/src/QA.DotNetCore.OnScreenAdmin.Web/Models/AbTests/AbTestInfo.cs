using QA.DotNetCore.Engine.Persistent.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DotNetCore.OnScreenAdmin.Web.Models.AbTests
{
    public class AbTestInfo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public IEnumerable<AbTestVariantInfo> Variants { get; set; }

        public AbTestInfo(AbTestPersistentData test, IEnumerable<AbTestContainerBasePersistentData> containers)
        {
            if (test != null && test.Percentage != null && containers != null && containers.Any())
            {
                Id = test.Id;
                Title = test.Title;
                Comment = test.Comment;
                StartDate = test.StartDate;
                EndDate = test.EndDate;

                var sum = test.Percentage.Sum();
                var vars = new List<AbTestVariantInfo>();
                for (var i = 0; i < test.Percentage.Length; i++)
                {
                    vars.Add(new AbTestVariantInfo(containers, i, Math.Round((100m * test.Percentage[i]) / sum, 2)));
                }
                Variants = vars;
            }
        }
    }

    public class AbTestVariantInfo
    {
        public int Choice { get; set; }
        public decimal Percent { get; set; }
        public IEnumerable<AbTestVariantInContainerInfo> Containers { get; set; }

        public AbTestVariantInfo(IEnumerable<AbTestContainerBasePersistentData> containers, int choice, decimal percent)
        {
            Choice = choice;
            Percent = percent;
            Containers = containers.Select(c => new AbTestVariantInContainerInfo(c, choice)).Where(c => c.VariantId > 0).ToArray();
        }
    }

    public class AbTestVariantInContainerInfo
    {
        public int Cid { get; set; }
        public string ContainerDescription { get; set; }
        public string VariantDescription { get; set; }
        public int VariantId { get; set; }
        public string Type { get; set; }

        public AbTestVariantInContainerInfo(AbTestContainerBasePersistentData container, int choice)
        {
            if (container is AbTestScriptContainerPersistentData)
            {
                var script = (container as AbTestScriptContainerPersistentData).Scripts.FirstOrDefault(s => s.VersionNumber == choice);
                if (script != null)
                {
                    VariantDescription = script.Description;
                    VariantId = script.Id;
                }
            }
            else if (container is AbTestClientRedirectContainerPersistentData)
            {
                var redirect = (container as AbTestClientRedirectContainerPersistentData).Redirects.FirstOrDefault(r => r.VersionNumber == choice);
                if (redirect != null)
                {
                    VariantDescription = $"Client redirect to {redirect.RedirectUrl}";
                    VariantId = redirect.Id;
                }
            }

            Cid = container.Id;
            ContainerDescription = container.Description;
            Type = container.Type.ToString();
        }
    }
}
