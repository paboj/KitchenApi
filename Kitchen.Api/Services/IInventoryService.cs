using Kitchen.Api.Models.Entities;

namespace Kitchen.Api.Services
{
    public interface IInventoryService
    {
        IEnumerable<Ingredient> GetAll();
        Ingredient? GetById(Guid id);
        void Add(Ingredient ingredient);
        bool Update(Guid id, Ingredient updatedIngredient);
        bool Delete(Guid id);
    }
}