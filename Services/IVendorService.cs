using InternInventory.Models;

namespace InternInventory.Services
{
    public interface IVendorService
    {
        Task<IEnumerable<Vendor>> GetAllVendorsAsync();
        Task<IEnumerable<Vendor>> SearchVendorsAsync(string searchTerm);
        Task<Vendor?> GetVendorByIdAsync(int id);
        Task<bool> AddVendorAsync(Vendor vendor, string currentUserName);
        Task<bool> UpdateVendorAsync(Vendor vendor);
        Task<bool> DeleteVendorAsync(int id);
    }
}
