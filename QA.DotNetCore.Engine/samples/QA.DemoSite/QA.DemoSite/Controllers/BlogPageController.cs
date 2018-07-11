using Microsoft.AspNetCore.Mvc;
using QA.DemoSite.Interfaces;
using QA.DemoSite.Models.Pages;
using QA.DemoSite.ViewModels;
using QA.DotNetCore.Engine.Routing;
using System.Linq;

namespace QA.DemoSite.Controllers
{
    public class BlogPageController : ContentControllerBase<BlogPage>
    {
        public BlogPageController(IBlogService blogService)
        {
            BlogService = blogService;
        }

        public IBlogService BlogService { get; }

        public IActionResult Index()
        {
            return View(PrepareListViewModel(CurrentItem));
        }

        public IActionResult Details(int id)
        {
            return View(PrepareDetailsViewModel(CurrentItem, id));
        }

        private BlogPageViewModel PrepareListViewModel(BlogPage blogPage)
        {
            var vm = new BlogPageViewModel { Header = blogPage.Title };
            vm.Items.AddRange(BlogService.GetAllPosts().Select(p => new BlogItemInListViewModel
            {
                Title = p.Title,
                Brief = p.Brief,
                Date = p.PostDate.ToString("dd.MM.yyyy"),
                CategoryName = p.Category?.Title,
                Image = p.Image,
                YoutubeVideoCode = p.YoutubeVideoCode,
                Url = blogPage.GetUrl() + "/details/" + p.Id
            }));
            return vm;
        }

        private BlogDetailsViewModel PrepareDetailsViewModel(BlogPage blogPage, int id)
        {
            var dto = BlogService.GetPost(id);

            return new BlogDetailsViewModel
            {
                Title = dto.Title,
                Date = dto.PostDate.ToString("dd.MM.yyyy"),
                CategoryName = dto.Category?.Title,
                Image = dto.Image,
                YoutubeVideoCode = dto.YoutubeVideoCode,
                Tags = dto.Tags.Select(t => t.Title).ToList(),
                Text = dto.Text
            };
        }
    }
}
