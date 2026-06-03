using InternInventory.Models;

namespace InternInventory.Repositories
{
    public interface IStockReceiptRepository : IRepository<StockReceipt>
    {
        Task<IEnumerable<StockReceipt>> GetAllWithRelationsAsync();
        Task<StockReceipt?> GetByIdWithRelationsAsync(int id);
        Task<IEnumerable<StockReceipt>> SearchWithRelationsAsync(string searchTerm);
    }
}
