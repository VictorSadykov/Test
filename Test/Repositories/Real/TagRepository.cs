using Microsoft.EntityFrameworkCore;
using Test.Models;
using Test.Repositories.Abstract;

namespace Test.Repositories.Real
{
    public class TagRepository : ITagRepository
    {
        private readonly DataContext _context;

        public TagRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Tag>> GetTags()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<bool> DeleteTag(Tag tag)
        {
            _context.Tags.Remove(tag);
            return await Save();
        }

        public async Task<Tag> GetTagById(int id)
        {
            return await _context.Tags
                .Where(t => t.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<Tag> GetTagByName(string name)
        {
            return await _context.Tags
                .Where(t => t.Name.Trim().ToLower() == name.Trim().ToLower())
                .SingleOrDefaultAsync();
        }

        public async Task<bool> Create(Tag tag)
        {
            await _context.AddAsync(tag);
            return await Save();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> TagExists(int id)
        {
            return await _context.Tags.AnyAsync(x => x.Id == id);
        }
    }
}
