using Kitchen.Api.Domain.Entities;
using Kitchen.Api.ValueObjects;

namespace Kitchen.Api.Repositories
{
    public class InMemoryIngredientRepository : IIngredientRepository
    {
        private readonly List<Ingredient> _ingredients = new();
        public IEnumerable<Ingredient> GetAll() => _ingredients;
        public Ingredient? GetByName(string name) => _ingredients.FirstOrDefault(i => i.Name == name);
        public void Add (Ingredient ingredient) => _ingredients.Add(ingredient);
        public void Delete(string name)
        {
            var ingredient = GetByName(name);
            if (ingredient != null) _ingredients.Remove(ingredient);
        }
    }
}
