using Kitchen.Core.Domain.Entities;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Core.Repositories
{
    public interface IStockItemRepository
    {
        IEnumerable<StockItem> GetAll();
        StockItem? GetById(Guid id);
        IEnumerable<StockItem> GetByName(string name);
        void Add(StockItem stockItem);
        void Update(StockItem stockItem);
        void Delete(Guid id);

        IEnumerable<StockItem> GetAllWithDetails();
        StockItem? GetByIdWithDetails(Guid id);
        IEnumerable<StockItem> GetByNameWithDetails(string name);
    }
}
