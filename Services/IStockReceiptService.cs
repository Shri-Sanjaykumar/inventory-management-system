using InternInventory.Models;

namespace InternInventory.Services
{
    public interface IStockReceiptService
    {
        Task<IEnumerable<StockReceipt>> GetAllStockReceiptsAsync();
        Task<IEnumerable<StockReceipt>> SearchStockReceiptsAsync(string searchTerm);
        Task<StockReceipt?> GetStockReceiptByIdAsync(int id);
        Task<bool> AddStockReceiptAsync(StockReceipt receipt, string currentUserName);
        Task<bool> UpdateStockReceiptAsync(StockReceipt receipt);
        Task<bool> DeleteStockReceiptAsync(int id);
    }
}
