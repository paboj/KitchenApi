using Kitchen.Api.Commands;
using Kitchen.Api.Domain.Entities;

namespace Kitchen.Api.Services
{
    public interface IInventoryService
    {
        IEnumerable<Ingredient> GetAll();
        Ingredient? GetByName(string name);
        void Add(AddToStockCommand command);
        void Update(ModifyInStockCommand command);
        void Delete(string name);
    }
}