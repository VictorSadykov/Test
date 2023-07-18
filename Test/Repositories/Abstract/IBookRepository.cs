using Test.Models;

namespace Test.Repositories.Abstract
{
    public interface IBookRepository
    {
        Task<ICollection<Book>> GetBooks();
        Task<Book> GetBookById(int bookId);
        Task<Book> GetBookByTitle(string title);
        Task<ICollection<Author>> GetAllAuthorsThatThisBookDoesntHave(int id);
        Task<bool> BookExists(int id);
        Task<bool> Save();
        Task<ICollection<Tag>> GetAllTagsThatThisBookDoesntHave(int id);
        Task<bool> Create(Book book);
        Task<bool> Delete(Book book);
        Task<bool> DeleteBooks(List<Book> books);
    }
}
