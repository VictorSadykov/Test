using Microsoft.EntityFrameworkCore;
using Test.Models;
using Test.Repositories.Abstract;

namespace Test.Repositories.Real
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly DataContext _context;

        public AuthorRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteAuthor(Author author)
        {
            _context.Authors.Remove(author);
            return await Save();
        }

        public async Task<ICollection<Author>> GetAuthors()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task<Author> GetAuthorById(int id)
        {
            return await _context.Authors
                .Where(a => a.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<ICollection<Book>?> GetAllBooksAuthorWrote(int authorId)
        {
            var author = await _context.Authors
                .Include(x => x.Books)
                .SingleOrDefaultAsync(x => x.Id == authorId);

            return author?.Books;
        }

        public async Task<Author> GetAuthorByName(string name)
        {
            return await _context.Authors
                .Where(a => a.Name.Trim().ToLower() == name.Trim().ToLower())
                .SingleOrDefaultAsync();
        }



        public async Task<bool> Create(Author author)
        {
            await _context.AddAsync(author);
            return await Save();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> AuthorExists(int id)
        {
            return await _context.Authors.AnyAsync(x => x.Id == id);
        }
    }
}
