using Microsoft.EntityFrameworkCore;
using System.Net;
using Test.Models;
using Test.Repositories.Abstract;

namespace Test.Repositories.Real
{
    public class BookRepository : IBookRepository
    {
        private readonly DataContext _context;

        public BookRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> BookExists(int bookId)
        {
            return await _context.Books.AnyAsync(x => x.Id == bookId);
        }

        public async Task<bool> Create(Book book)
        {
            await _context.AddAsync(book);
            return await Save();
        }

        public async Task<bool> Delete(Book book)
        {
            _context.Books.Remove(book);
            return await Save();
        }

        public async Task<bool> DeleteBooks(List<Book> books)
        {
            _context.RemoveRange(books);
            return await Save();
        }

       
        public async Task<ICollection<Author>> GetAllAuthorsThatThisBookDoesntHave(int id)
        {
            return await _context.Authors
                .Where(a => a.Books.All(
                        b => b.Id != id
                    ))
                .ToListAsync();
        }

        public async Task<ICollection<Tag>> GetAllTagsThatThisBookDoesntHave(int id)
        {
            return await _context.Tags
                .Where(a => a.Books.All(
                        b => b.Id != id
                    ))
                .ToListAsync();
        }

        public async Task<Book> GetBookById(int bookId)
        {
            return await _context.Books
                .Where(b=> b.Id == bookId)
                .Include(b => b.Authors)
                .Include(b => b.Tags)
                .SingleOrDefaultAsync();
        }

        public async Task<Book> GetBookByTitle(string title)
        {
            return await _context.Books
                .Where(b => b.Title.Trim().ToLower() == title.Trim().ToLower())
                .SingleOrDefaultAsync();
        }

        public async Task<ICollection<Book>> GetBooks()
        {
            return await _context.Books.ToListAsync();
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}
