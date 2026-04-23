using Kitchen.Api.Domain.Entities;
using Kitchen.Api.ValueObjects;

namespace Kitchen.Api.Repositories
{
    public interface IIngredientRepository
    {
        IEnumerable<Ingredient> GetAll();
        Ingredient? GetByName(string name);
        void Add(Ingredient ingredient);
        void Delete(string name);
    }
}
