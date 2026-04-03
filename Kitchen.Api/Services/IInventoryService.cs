using Kitchen.Api.Models.Entities;

namespace Kitchen.Api.Services
{
    public interface IInventoryService
    {
        IEnumerable<Ingredient> GetAllIngredients();
        void AddOrUpdateIngredient(Ingredient ingredient);
    }
}