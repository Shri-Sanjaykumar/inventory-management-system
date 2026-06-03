using InternInventory.Models;
using InternInventory.Repositories;

namespace InternInventory.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;

        public ItemService(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            var items = await _itemRepository.GetAllAsync();
            return items.OrderBy(i => i.ItemName);
        }

        public async Task<IEnumerable<Item>> SearchItemsAsync(string searchTerm)
        {
            var items = await _itemRepository.SearchByItemNameAsync(searchTerm);
            return items.OrderBy(i => i.ItemName);
        }

        public async Task<Item?> GetItemByIdAsync(int id)
        {
            return await _itemRepository.GetByIdAsync(id);
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            item.ItemName = item.ItemName.Trim();
            if (item.DetailedDescription != null)
            {
                item.DetailedDescription = item.DetailedDescription.Trim();
            }

            await _itemRepository.AddAsync(item);
            await _itemRepository.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var existing = await _itemRepository.GetByIdAsync(item.ItemID);
            if (existing == null) return false;

            existing.ItemName = item.ItemName.Trim();
            existing.UnitOfMeasure = item.UnitOfMeasure.Trim();
            existing.OpeningBalance = item.OpeningBalance;
            existing.DetailedDescription = item.DetailedDescription?.Trim();

            await _itemRepository.UpdateAsync(existing);
            await _itemRepository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var existing = await _itemRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _itemRepository.DeleteAsync(id);
            await _itemRepository.SaveAsync();
            return true;
        }
    }
}
