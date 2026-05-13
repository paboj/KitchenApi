using Kitchen.Core.Domain.Entities;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Core.Repositories
{
    public interface IStockItemRepository
    {
        IEnumerable<StockItem> GetAll();
        StockItem? GetByName(string name);
        void Add(StockItem stockItem);
        void Update(StockItem stockItem);
        void Delete(string name);

        IEnumerable<StockItem> GetAllWithDetails();
        StockItem? GetByNameWithDetails(string name);
    }
}
