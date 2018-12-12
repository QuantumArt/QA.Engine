using QA.DemoSite.Interfaces.Dto;
using System.Collections.Generic;

namespace QA.DemoSite.Interfaces
{
    public interface IBlogService
    {
        IEnumerable<BlogPostDto> GetAllPosts();
        BlogPostDto GetPost(int id);
    }
}
