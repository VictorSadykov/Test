using Test.Models;

namespace Test.Repositories.Abstract
{
    public interface IAuthorRepository
    {
        Task<bool> Save();
        Task<bool> AuthorExists(int id);
        Task<bool> Create(Author author);
        Task<Author> GetAuthorById(int id);
        Task<Author> GetAuthorByName(string name);
        Task<ICollection<Author>> GetAuthors();
        Task<ICollection<Book>> GetAllBooksAuthorWrote(int authorId);
        Task<bool> DeleteAuthor(Author author);
    }
}
