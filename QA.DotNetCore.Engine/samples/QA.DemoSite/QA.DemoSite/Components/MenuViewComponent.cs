using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QA.DotNetCore.Engine.Routing;
using QA.DemoSite.Models;
using QA.DotNetCore.Engine;
using QA.DemoSite.Models.Pages;
using QA.DotNetCore.Engine.QpData;

namespace QA.DemoSite.Components
{
    public class MenuViewComponent : ViewComponent
    {
        private const int MenuDepth = 3;
        public async Task<IViewComponentResult> InvokeAsync()
        {
            await Task.Yield();

            var startPage = ViewContext.GetStartPage();
            if (startPage == null) return null;

            var model = new MenuViewModel();

            var topLevelItems = startPage.GetChildren().Where(w => w.IsPage).OfType<AbstractPage>().OrderBy(o => o.SortOrder);

            var ci = ViewContext.GetCurrentItem<AbstractPage>();

            foreach (var tlitem in topLevelItems)
            {
                var resultBuildMenu = BuildMenu(tlitem, MenuDepth, ci.Id);
                model.Items.Add(new MenuItem
                {
                    Title = tlitem.Title,
                    Alias = tlitem.Alias,
                    Href = tlitem.GetUrl(),
                    Children = resultBuildMenu,
                    IsActive = tlitem.Id == ci.Id || resultBuildMenu.Where(w => w.IsActive).Any()
                });
            }

            model.Items = model.Items?.OrderBy(o => o.Order).ToList();

            return View(model);
        }


        private static List<MenuItem> BuildMenu(AbstractPage item, int level, int currentId)
        {
            if (level <= 0)
            {
                return null;
            }

            var itemList = new List<MenuItem>();
            foreach (var itemlv in item.GetChildren().Where(w => w.IsPage).OfType<AbstractPage>().OrderBy(o => o.SortOrder))
            {

                var resultBuidMenu = BuildMenu(itemlv, level - 1, currentId);
                itemList.Add(new MenuItem
                {
                    Title = itemlv.Title,
                    Alias = itemlv.Alias,
                    Href = itemlv.GetUrl(),
                    Children = resultBuidMenu,
                    IsActive = itemlv.Id == currentId || resultBuidMenu.Where(w => w.IsActive).Any()
                });
            }
            return itemList;
        }
    }
}
