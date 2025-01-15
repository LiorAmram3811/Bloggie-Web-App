using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly BloggieDbContext _bloggieDbContext;

        public BlogPostRepository(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbContext = bloggieDbContext;
        }
        public async Task<BlogPost> AddAsync(BlogPost BlogPost)
        {
            await _bloggieDbContext.BlogPosts.AddAsync(BlogPost);
            await _bloggieDbContext.SaveChangesAsync();
            return BlogPost;
        }
        public async Task<IEnumerable<BlogPost>> GetAllAsync()
        {
            return await _bloggieDbContext.BlogPosts.Include(x => x.Tags).ToListAsync();
        }
        public async Task<BlogPost?> GetAsync(Guid id)
        {
            //var BlogPost = _bloggieDbContext.BlogPosts.Find(id);

            var BlogPost = await _bloggieDbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(t => t.Id == id);
            return BlogPost;
        }
        public async Task<BlogPost?> UpdateAsync(BlogPost blogPost)
        {
            var updatedBlogPost = await _bloggieDbContext.BlogPosts.Include(x => x.Tags)
                .FirstOrDefaultAsync(t => t.Id == blogPost.Id);

            if (updatedBlogPost != null)
            {
                updatedBlogPost.Id = blogPost.Id;
                updatedBlogPost.Heading = blogPost.Heading;
                updatedBlogPost.PageTitle = blogPost.PageTitle;
                updatedBlogPost.Content = blogPost.Content;
                updatedBlogPost.ShortDescription = blogPost.ShortDescription;
                updatedBlogPost.FeaturedImageUrl = blogPost.FeaturedImageUrl;
                updatedBlogPost.UrlHandle = blogPost.UrlHandle;
                updatedBlogPost.PublishedDate = blogPost.PublishedDate;
                updatedBlogPost.Author = blogPost.Author;
                updatedBlogPost.IsVisible = blogPost.IsVisible;
                updatedBlogPost.Tags = blogPost.Tags;
                await _bloggieDbContext.SaveChangesAsync();
                return updatedBlogPost;
            }
            return null;
        }
        public async Task<BlogPost?> DeleteAsync(Guid id)
        {
            var DeletedBlogPost = await _bloggieDbContext.BlogPosts.FindAsync(id);

            if (DeletedBlogPost != null)
            {
                _bloggieDbContext.BlogPosts.Remove(DeletedBlogPost);
                await _bloggieDbContext.SaveChangesAsync();
                return DeletedBlogPost;
            }
            return null;
        }

        public async Task<BlogPost?> GetByUrlHandleAsync(string urlHandle)
        {
            var blogPost =  await _bloggieDbContext.BlogPosts.Include(x => x.Tags).FirstOrDefaultAsync(t => t.UrlHandle == urlHandle);
            return blogPost;
        }
    }
}
