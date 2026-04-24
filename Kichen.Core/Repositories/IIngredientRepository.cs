using Kitchen.Core.Domain.Entities;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Core.Repositories
{
    public interface IIngredientRepository
    {
        IEnumerable<Ingredient> GetAll();
        Ingredient? GetByName(string name);
        void Add(Ingredient ingredient);
        void Delete(string name);
    }
}
