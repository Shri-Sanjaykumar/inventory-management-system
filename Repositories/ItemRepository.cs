using Microsoft.EntityFrameworkCore;
using InternInventory.Data;
using InternInventory.Models;

namespace InternInventory.Repositories
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(InventoryDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Item>> SearchByItemNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync();
            }

            var lowerTerm = searchTerm.ToLower();
            return await _dbSet.Where(i => 
                i.ItemName.ToLower().Contains(lowerTerm) || 
                i.UnitOfMeasure.ToLower().Contains(lowerTerm)
            ).ToListAsync();
        }
    }
}
