using Kitchen.Api.Models.Entities;

namespace Kitchen.Api.Services
{
    public interface IIngredientCatalogService
    {
        IEnumerable<IngredientDefinition> GetAll();
        IngredientDefinition? GetByName(string name);
        void Add(IngredientDefinition ingredientDefinition);
        bool Update(IngredientDefinition ingredientDefinition);
        bool Delete(string name);
    }
}
