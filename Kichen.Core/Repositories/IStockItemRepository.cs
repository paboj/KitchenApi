using Kitchen.Core.Domain.Entities;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Core.Repositories
{
    public interface IStockItemRepository
    {
        Task<IEnumerable<StockItem>> GetAll();
        Task<StockItem?> GetById(Guid id);
        Task<IEnumerable<StockItem>> GetByName(string name);
        Task Add(StockItem stockItem);
        Task Update(StockItem stockItem);
        Task Delete(Guid id);

        Task<IEnumerable<StockItem>> GetAllWithDetails();
        Task<StockItem?> GetByIdWithDetails(Guid id);
        Task<IEnumerable<StockItem>> GetByNameWithDetails(string name);
    }
}
