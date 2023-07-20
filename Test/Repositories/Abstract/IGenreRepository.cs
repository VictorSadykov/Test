using Test.Models;

namespace Test.Repositories.Abstract
{
    public interface IGenreRepository
    {
        Task<bool> Save();
        Task<bool> Create(Genre tag);
        Task<bool> TagExists(int id);
        Task<Genre> GetTagById(int id);
        Task<ICollection<Genre>> GetTags();
        Task<Genre> GetTagByName(string name);
    }
}
