using Test.Models;

namespace Test.Repositories.Abstract
{
    public interface ITagRepository
    {
        Task<bool> Save();
        Task<bool> Create(Tag tag);
        Task<bool> TagExists(int id);
        Task<Tag> GetTagById(int id);
        Task<ICollection<Tag>> GetTags();
        Task<Tag> GetTagByName(string name);
    }
}
