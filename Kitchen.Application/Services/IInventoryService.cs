using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;

namespace Kitchen.Application.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<StockItem>> GetAll();
        Task<StockItem?> GetById(Guid id);
        Task<IEnumerable<StockItem>> GetByName(string name);
        Task Add(AddStockItemCommand command);
        Task Update(ModifyStockItemCommand command);
        Task Delete(Guid id);
    }
}