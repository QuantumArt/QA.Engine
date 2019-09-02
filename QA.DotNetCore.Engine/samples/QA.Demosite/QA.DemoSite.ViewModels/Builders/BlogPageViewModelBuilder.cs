using QA.DemoSite.Interfaces;
using QA.DotNetCore.Engine.Abstractions;
using System.Linq;

namespace QA.DemoSite.ViewModels.Builders
{
    public class BlogPageViewModelBuilder
    {
        public IBlogService BlogService { get; }

        public BlogPageViewModelBuilder(IBlogService blogService)
        {
            BlogService = blogService;
        }

        public BlogPageViewModel BuildList(IAbstractPage blogPage)
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

        public BlogDetailsViewModel BuildDetails(IAbstractPage blogPage, int id)
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
