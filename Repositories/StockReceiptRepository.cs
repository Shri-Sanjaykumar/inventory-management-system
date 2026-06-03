using Microsoft.EntityFrameworkCore;
using InternInventory.Data;
using InternInventory.Models;

namespace InternInventory.Repositories
{
    public class StockReceiptRepository : Repository<StockReceipt>, IStockReceiptRepository
    {
        public StockReceiptRepository(InventoryDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<StockReceipt>> GetAllWithRelationsAsync()
        {
            return await _dbSet
                .Include(s => s.Vendor)
                .Include(s => s.Project)
                .Include(s => s.Item)
                .OrderByDescending(s => s.CreatedOn)
                .ToListAsync();
        }

        public async Task<StockReceipt?> GetByIdWithRelationsAsync(int id)
        {
            return await _dbSet
                .Include(s => s.Vendor)
                .Include(s => s.Project)
                .Include(s => s.Item)
                .FirstOrDefaultAsync(s => s.StockReceiptID == id);
        }

        public async Task<IEnumerable<StockReceipt>> SearchWithRelationsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllWithRelationsAsync();
            }

            var lowerTerm = searchTerm.ToLower();
            return await _dbSet
                .Include(s => s.Vendor)
                .Include(s => s.Project)
                .Include(s => s.Item)
                .Where(s => 
                    s.Vendor!.FirstName.ToLower().Contains(lowerTerm) || 
                    s.Vendor!.LastName.ToLower().Contains(lowerTerm) ||
                    s.Project!.ProjectName.ToLower().Contains(lowerTerm) ||
                    s.Item!.ItemName.ToLower().Contains(lowerTerm) ||
                    s.CreatedBy.ToLower().Contains(lowerTerm)
                )
                .OrderByDescending(s => s.CreatedOn)
                .ToListAsync();
        }
    }
}
