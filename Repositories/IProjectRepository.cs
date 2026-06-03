using InternInventory.Models;

namespace InternInventory.Repositories
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<IEnumerable<Project>> SearchByProjectNameAsync(string searchTerm);
    }
}
