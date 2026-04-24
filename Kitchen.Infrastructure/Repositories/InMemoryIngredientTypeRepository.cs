using Kitchen.Core.Domain.Entities;
using Kitchen.Core.Repositories;

namespace Kitchen.Infrastructure.Repositories
{
    internal class InMemoryIngredientTypeRepository : IIngredientTypeRepository
    {
        private readonly List<IngredientType> _ingredientTypes = new();
        public IEnumerable<IngredientType> GetAll() => _ingredientTypes;
        public IngredientType? GetByName(string name) => _ingredientTypes.FirstOrDefault(i => i.Name == name);
        public void Add (IngredientType ingredient) => _ingredientTypes.Add(ingredient);
        public void Delete(string name)
        {
            var ingredientType = GetByName(name);
            if (ingredientType != null) _ingredientTypes.Remove(ingredientType);
        }
    }
}
