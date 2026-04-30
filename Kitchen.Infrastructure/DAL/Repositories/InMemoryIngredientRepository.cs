using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Repositories;

namespace Kitchen.Infrastructure.DAL.Repositories
{
    internal class InMemoryIngredientRepository : IIngredientRepository
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
