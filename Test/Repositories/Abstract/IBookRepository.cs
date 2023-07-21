using Test.Models;

namespace Test.Repositories.Abstract
{
    public interface IBookRepository
    {
        Task<ICollection<Book>> GetBooks();
        Task<Book> GetBookById(int bookId);
        Task<Book> GetBookByTitle(string title);
        Task<bool> BookExists(int id);
        Task<bool> Save();
        Task<bool> Create(Book book);
        Task<bool> Delete(Book book);
        Task<bool> DeleteBooks(List<Book> books);
        ICollection<Book> GetBooksWithLimitAndPage(List<Book> books, int? limit, int? page);
        Task<bool> Update(Book book);
    }
}
