using InternInventory.Models;

namespace InternInventory.Services
{
    public interface IItemService
    {
        Task<IEnumerable<Item>> GetAllItemsAsync();
        Task<IEnumerable<Item>> SearchItemsAsync(string searchTerm);
        Task<Item?> GetItemByIdAsync(int id);
        Task<bool> AddItemAsync(Item item);
        Task<bool> UpdateItemAsync(Item item);
        Task<bool> DeleteItemAsync(int id);
    }
}
