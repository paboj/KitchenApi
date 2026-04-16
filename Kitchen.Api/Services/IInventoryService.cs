using Kitchen.Api.Commands;
using Kitchen.Api.Domain.Entities;

namespace Kitchen.Api.Services
{
    public interface IInventoryService
    {
        IEnumerable<Ingredient> GetAll();
        Ingredient? GetByName(string name);
        void Add(AddToStockCommand command);
        bool Update(ModifyIngredientCommand command);
        bool Delete(string name);
    }
}