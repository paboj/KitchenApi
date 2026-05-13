using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;

namespace Kitchen.Application.Services
{
    public interface IInventoryService
    {
        IEnumerable<StockItem> GetAll();
        StockItem? GetByName(string name);
        void Add(AddToStockCommand command);
        void Update(ModifyInStockCommand command);
        void Delete(string name);
    }
}