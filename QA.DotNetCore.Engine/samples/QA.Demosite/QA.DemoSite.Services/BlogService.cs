using Microsoft.EntityFrameworkCore;
using QA.DemoSite.DAL;
using QA.DemoSite.Interfaces;
using QA.DemoSite.Interfaces.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QA.DemoSite.Services
{
    public class BlogService : IBlogService
    {
        public BlogService(QpDataContext qpDataContext)
        {
            QpDataContext = qpDataContext;
        }

        public QpDataContext QpDataContext { get; }

        public IEnumerable<BlogPostDto> GetAllPosts()
        {
            return QpDataContext.BlogPosts
                .Include(c => c.Category)
                .Include(t => t.Tags)
                .ThenInclude(ti => ti.BlogTagLinkedItem).ToList()
                .Select(Map).ToList();
        }

        public BlogPostDto GetPost(int id)
        {
            var post = QpDataContext.BlogPosts
                .Include(c=>c.Category)
                .Include(t=>t.Tags)
                .ThenInclude(ti=>ti.BlogTagLinkedItem)
                .FirstOrDefault(bp => bp.Id == id);
            return Map(post);
        }

        private BlogPostDto Map(BlogPost blogPost)
        {
            if (blogPost == null)
                return null;

            return new BlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Brief = blogPost.Brief,
                Category = Map(blogPost.Category),
                Image = blogPost.ImageUrl,
                PostDate = blogPost.PostDate.GetValueOrDefault(new DateTime(2001, 01, 01)),
                Text = blogPost.Text,
                YoutubeVideoCode = blogPost.YoutubeVideoCode,
                Tags = blogPost.Tags.Select(Map).ToList()
            };
        }

        private BlogCategoryDto Map(BlogCategory blogCategory)
        {
            if (blogCategory == null)
                return null;

            return new BlogCategoryDto
            {
                Id = blogCategory.Id,
                Title = blogCategory.Title,
                SortOrder = blogCategory.SortOrder
            };
        }

        private BlogTagDto Map(BlogPost2BlogTagForTags blogTag)
        {
            if (blogTag == null || blogTag.BlogTagLinkedItem == null)
                return null;

            return new BlogTagDto
            {
                Id = blogTag.BlogTagLinkedItem.Id,
                Title = blogTag.BlogTagLinkedItem.Title
            };
        }
    }
}
