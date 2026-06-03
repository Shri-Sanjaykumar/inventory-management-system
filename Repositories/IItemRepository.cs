using InternInventory.Models;

namespace InternInventory.Repositories
{
    public interface IItemRepository : IRepository<Item>
    {
        Task<IEnumerable<Item>> SearchByItemNameAsync(string searchTerm);
    }
}
