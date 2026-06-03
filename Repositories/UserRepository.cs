using Microsoft.EntityFrameworkCore;
using InternInventory.Data;
using InternInventory.Models;

namespace InternInventory.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(InventoryDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }
    }
}
