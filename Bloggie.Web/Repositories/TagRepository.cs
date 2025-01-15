using Bloggie.Web.Data;
using Bloggie.Web.Models.Domain;
using Bloggie.Web.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Bloggie.Web.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly BloggieDbContext _bloggieDbContext;

        public TagRepository(BloggieDbContext bloggieDbContext)
        {
            _bloggieDbContext = bloggieDbContext;
        }
        public async Task<Tag> AddAsync(Tag tag)
        {
            await _bloggieDbContext.Tags.AddAsync(tag);
            await _bloggieDbContext.SaveChangesAsync();
            return tag;
        }
        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            return await _bloggieDbContext.Tags.ToListAsync();
        }
        public async Task<Tag?> GetAsync(Guid id)
        {
            //var tag = _bloggieDbContext.Tags.Find(id);

            var tag = await _bloggieDbContext.Tags.FirstOrDefaultAsync(t => t.Id == id);
            return tag;
        }
        public async Task<Tag?> UpdateAsync(Tag tag)
        {
            var updatedTag = await _bloggieDbContext.Tags.FindAsync(tag.Id);

            if (updatedTag != null)
            {
                updatedTag.Name = tag.Name;
                updatedTag.DisplayName = tag.DisplayName;
                await _bloggieDbContext.SaveChangesAsync();
                return updatedTag;
            }
            return null;
        }
        public async Task<Tag?> DeleteAsync(Guid id)
        {
            var DeletedTag = await _bloggieDbContext.Tags.FindAsync(id);

            if (DeletedTag != null)
            {
                _bloggieDbContext.Tags.Remove(DeletedTag);
                await _bloggieDbContext.SaveChangesAsync();
                return DeletedTag;
            }
            return null;
        }
    }
}
