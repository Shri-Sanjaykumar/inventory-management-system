using InternInventory.Models;

namespace InternInventory.Repositories
{
    public interface IVendorRepository : IRepository<Vendor>
    {
        Task<IEnumerable<Vendor>> SearchByNameOrEmailAsync(string searchTerm);
    }
}
