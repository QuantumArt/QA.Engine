using Microsoft.EntityFrameworkCore;
using QA.DemoSite.Mssql.DAL;
using QA.DemoSite.Postgre.DAL;
using QA.DemoSite.Interfaces;
using QA.DemoSite.Interfaces.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using QA.DotNetCore.Engine.Persistent.Interfaces;
using QA.DotNetCore.Engine.Persistent.Interfaces.Settings;

namespace QA.DemoSite.Services
{
    public class BlogService : IBlogService
    {
        public BlogService(QpSettings qpSettings, IDbContext context)
        {
            QpDataContext = context;
            if (!Enum.TryParse(qpSettings.DatabaseType, true, out DatabaseType dbType))
            {
                dbType = DatabaseType.SqlServer;
            }
        }

        readonly DatabaseType dbType = DatabaseType.SqlServer;
        public IDbContext QpDataContext { get; }

        public IEnumerable<BlogPostDto> GetAllPosts()
        {
            if (dbType == DatabaseType.Postgres)
            {
                return (QpDataContext as PostgreQpDataContext).BlogPosts
                  .Include(c => c.Category)
                  .Include(t => t.Tags)
                  .ThenInclude(ti => ti.BlogTagLinkedItem).ToList()
                  .Select(Map).ToArray();
            }
            return (QpDataContext as QpDataContext).BlogPosts
               .Include(c => c.Category)
               .Include(t => t.Tags)
               .ThenInclude(ti => ti.BlogTagLinkedItem).ToList()
               .Select(Map).ToArray();
        }

        public BlogPostDto GetPost(int id)
        {
            if (dbType == DatabaseType.Postgres)
            {
                return Map((QpDataContext as PostgreQpDataContext).BlogPosts
                 .Include(c => c.Category)
                 .Include(t => t.Tags)
                 .ThenInclude(ti => ti.BlogTagLinkedItem)
                 .FirstOrDefault(bp => bp.Id == id));
            }

            return Map((QpDataContext as QpDataContext).BlogPosts
            .Include(c => c.Category)
            .Include(t => t.Tags)
            .ThenInclude(ti => ti.BlogTagLinkedItem)
            .FirstOrDefault(bp => bp.Id == id));
        }



        private BlogPostDto Map(Mssql.DAL.BlogPost blogPost)
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

        private BlogPostDto Map(Postgre.DAL.BlogPost blogPost)
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

        private BlogCategoryDto Map(Mssql.DAL.BlogCategory blogCategory)
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

        private BlogCategoryDto Map(Postgre.DAL.BlogCategory blogCategory)
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

        private BlogTagDto Map(Mssql.DAL.BlogPost2BlogTagForTags blogTag)
        {
            if (blogTag == null || blogTag.BlogTagLinkedItem == null)
                return null;

            return new BlogTagDto
            {
                Id = blogTag.BlogTagLinkedItem.Id,
                Title = blogTag.BlogTagLinkedItem.Title
            };
        }
        private BlogTagDto Map(Postgre.DAL.BlogPost2BlogTagForTags blogTag)
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
