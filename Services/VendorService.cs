using InternInventory.Models;
using InternInventory.Repositories;

namespace InternInventory.Services
{
    public class VendorService : IVendorService
    {
        private readonly IVendorRepository _vendorRepository;

        public VendorService(IVendorRepository vendorRepository)
        {
            _vendorRepository = vendorRepository;
        }

        public async Task<IEnumerable<Vendor>> GetAllVendorsAsync()
        {
            var vendors = await _vendorRepository.GetAllAsync();
            return vendors.OrderBy(v => v.FirstName);
        }

        public async Task<IEnumerable<Vendor>> SearchVendorsAsync(string searchTerm)
        {
            var vendors = await _vendorRepository.SearchByNameOrEmailAsync(searchTerm);
            return vendors.OrderBy(v => v.FirstName);
        }

        public async Task<Vendor?> GetVendorByIdAsync(int id)
        {
            return await _vendorRepository.GetByIdAsync(id);
        }

        public async Task<bool> AddVendorAsync(Vendor vendor, string currentUserName)
        {
            vendor.CreatedBy = currentUserName;
            vendor.CreatedOn = DateTime.UtcNow;

            await _vendorRepository.AddAsync(vendor);
            await _vendorRepository.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateVendorAsync(Vendor vendor)
        {
            var existing = await _vendorRepository.GetByIdAsync(vendor.VendorID);
            if (existing == null) return false;

            // Update editable fields
            existing.FirstName = vendor.FirstName.Trim();
            existing.LastName = vendor.LastName.Trim();
            existing.AddressLine1 = vendor.AddressLine1.Trim();
            existing.City = vendor.City.Trim();
            existing.State = vendor.State.Trim();
            existing.Pincode = vendor.Pincode.Trim();
            existing.Email = vendor.Email.Trim();
            existing.PhoneNumber = vendor.PhoneNumber.Trim();

            await _vendorRepository.UpdateAsync(existing);
            await _vendorRepository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteVendorAsync(int id)
        {
            var existing = await _vendorRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _vendorRepository.DeleteAsync(id);
            await _vendorRepository.SaveAsync();
            return true;
        }
    }
}
