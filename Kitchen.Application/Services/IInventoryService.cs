using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;

namespace Kitchen.Application.Services
{
    public interface IInventoryService
    {
        IEnumerable<StockItem> GetAll();
        StockItem? GetById(Guid id);
        IEnumerable<StockItem> GetByName(string name);
        void Add(AddStockItemCommand command);
        void Update(ModifyStockItemCommand command);
        void Delete(Guid id);
    }
}