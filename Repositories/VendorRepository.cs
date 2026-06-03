using Microsoft.EntityFrameworkCore;
using InternInventory.Data;
using InternInventory.Models;

namespace InternInventory.Repositories
{
    public class VendorRepository : Repository<Vendor>, IVendorRepository
    {
        public VendorRepository(InventoryDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Vendor>> SearchByNameOrEmailAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync();
            }

            var lowerTerm = searchTerm.ToLower();
            return await _dbSet.Where(v => 
                v.FirstName.ToLower().Contains(lowerTerm) || 
                v.LastName.ToLower().Contains(lowerTerm) || 
                v.Email.ToLower().Contains(lowerTerm) ||
                v.City.ToLower().Contains(lowerTerm)
            ).ToListAsync();
        }
    }
}
