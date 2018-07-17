using QA.DotNetCore.Engine.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QA.DemoSite.ViewModels
{
    public class BreadcrumbsViewModel
    {
        public BreadcrumbsViewModel(IAbstractItem currentPage, string additionalBreadcrumb)
        {
            var result = new List<BreadcrumbsItemViewModel>();
            if (additionalBreadcrumb != null)
                result.Add(new BreadcrumbsItemViewModel { Title = additionalBreadcrumb });

            while (!(currentPage is IStartPage))
            {
                if (currentPage.IsPage)
                    result.Add(new BreadcrumbsItemViewModel { Title = currentPage.Title, Url = currentPage.GetUrl() });
                currentPage = currentPage.Parent;
            }
            if (result.Any())
            {
                result.First().IsActive = true;
                result.First().Url = null;
            }
            result.Reverse();
            Items = result;
        }

        public IList<BreadcrumbsItemViewModel> Items { get; private set; }
    }

    public class BreadcrumbsItemViewModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }
        public bool IsLink { get { return !String.IsNullOrWhiteSpace(Url); } }
    }
}
