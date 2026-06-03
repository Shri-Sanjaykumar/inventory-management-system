using Microsoft.EntityFrameworkCore;
using InternInventory.Data;
using InternInventory.Models;

namespace InternInventory.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(InventoryDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Project>> SearchByProjectNameAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetAllAsync();
            }

            var lowerTerm = searchTerm.ToLower();
            return await _dbSet.Where(p => 
                p.ProjectName.ToLower().Contains(lowerTerm) || 
                p.City.ToLower().Contains(lowerTerm) ||
                p.State.ToLower().Contains(lowerTerm)
            ).ToListAsync();
        }
    }
}
