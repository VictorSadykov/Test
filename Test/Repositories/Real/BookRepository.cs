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

       
        

        public async Task<Book> GetBookById(int bookId)
        {
            return await _context.Books
                .Include(x => x.Genre)
                .Include(x => x.Author)
                .Where(b=> b.Id == bookId)                
                .SingleOrDefaultAsync();
        }

        public async Task<Book> GetBookByTitle(string title)
        {
            return await _context.Books
                .Include(x => x.Genre)
                .Include(x => x.Author)
                .Where(b => b.Title.Trim().ToLower() == title.Trim().ToLower())
                .SingleOrDefaultAsync();
        }

        public async Task<ICollection<Book>> GetBooks()
        {
            return await _context.Books
                .Include(x => x.Genre)
                .Include(x => x.Author)
                .ToListAsync();
        }

        public ICollection<Book> GetBooksWithLimitAndPage(List<Book> books, int? limit, int? page)
        {
            int newLimit = limit ?? 9;
            int newPage = page ?? 1;

            int startIndex = (newPage - 1) * newLimit;
            int endIndex = startIndex + newLimit - 1;

            if (endIndex >= books.Count)
            {
                endIndex = books.Count - 1;
            }

            List<Book> booksPagedLimited = new List<Book>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                booksPagedLimited.Add(books[i]);
            }

            return booksPagedLimited;
        }

        public async Task<bool> Save()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }

        public async Task<bool> Update(Book book)
        {
            var saved = _context.Update(book);
            return await Save();
        }
    }
}
