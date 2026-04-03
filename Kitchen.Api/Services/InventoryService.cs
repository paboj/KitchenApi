using Kitchen.Api.Models.Entities;

namespace Kitchen.Api.Services
{
    public class InventoryService : IInventoryService
    {
        // temp list before database
        private readonly List<Ingredient> _ingredients = new();

        public IEnumerable<Ingredient> GetAllIngredients()
        {
            return _ingredients;
        }

        public void AddOrUpdateIngredient(Ingredient ingredient)
        {
            if (ingredient.Id == Guid.Empty)
            {
                ingredient.Id = Guid.NewGuid();
            }

            var existing = _ingredients.FirstOrDefault(x => x.Id == ingredient.Id);
            if (existing != null)
            {
                _ingredients.Remove(existing);
            }

            _ingredients.Add(ingredient);
        }
    }
}