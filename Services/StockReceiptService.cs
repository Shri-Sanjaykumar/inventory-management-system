using InternInventory.Models;
using InternInventory.Repositories;

namespace InternInventory.Services
{
    public class StockReceiptService : IStockReceiptService
    {
        private readonly IStockReceiptRepository _stockReceiptRepository;

        public StockReceiptService(IStockReceiptRepository stockReceiptRepository)
        {
            _stockReceiptRepository = stockReceiptRepository;
        }

        public async Task<IEnumerable<StockReceipt>> GetAllStockReceiptsAsync()
        {
            return await _stockReceiptRepository.GetAllWithRelationsAsync();
        }

        public async Task<IEnumerable<StockReceipt>> SearchStockReceiptsAsync(string searchTerm)
        {
            return await _stockReceiptRepository.SearchWithRelationsAsync(searchTerm);
        }

        public async Task<StockReceipt?> GetStockReceiptByIdAsync(int id)
        {
            return await _stockReceiptRepository.GetByIdWithRelationsAsync(id);
        }

        public async Task<bool> AddStockReceiptAsync(StockReceipt receipt, string currentUserName)
        {
            // Business Rule: Quantity must be > 0
            if (receipt.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than 0.");
            }

            // Business Rule: Receipt Date must not be greater than current date
            // Using DateTime.Today to support local date validation
            if (receipt.ReceiptDate.Date > DateTime.Today)
            {
                throw new ArgumentException("Receipt Date cannot be in the future.");
            }

            receipt.CreatedBy = currentUserName;
            receipt.CreatedOn = DateTime.UtcNow;

            await _stockReceiptRepository.AddAsync(receipt);
            await _stockReceiptRepository.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateStockReceiptAsync(StockReceipt receipt)
        {
            var existing = await _stockReceiptRepository.GetByIdAsync(receipt.StockReceiptID);
            if (existing == null) return false;

            if (receipt.Quantity <= 0)
            {
                throw new ArgumentException("Quantity must be greater than 0.");
            }

            if (receipt.ReceiptDate.Date > DateTime.Today)
            {
                throw new ArgumentException("Receipt Date cannot be in the future.");
            }

            // Update editable properties
            existing.VendorID = receipt.VendorID;
            existing.ProjectID = receipt.ProjectID;
            existing.ItemID = receipt.ItemID;
            existing.ReceiptDate = receipt.ReceiptDate;
            existing.Quantity = receipt.Quantity;

            await _stockReceiptRepository.UpdateAsync(existing);
            await _stockReceiptRepository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteStockReceiptAsync(int id)
        {
            var existing = await _stockReceiptRepository.GetByIdAsync(id);
            if (existing == null) return false;

            await _stockReceiptRepository.DeleteAsync(id);
            await _stockReceiptRepository.SaveAsync();
            return true;
        }
    }
}
