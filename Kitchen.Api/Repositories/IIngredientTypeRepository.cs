using Kitchen.Api.Domain.Entities;
using Kitchen.Api.ValueObjects;

namespace Kitchen.Api.Repositories
{
    public interface IIngredientTypeRepository
    {
        IEnumerable<IngredientType> GetAll();
        IngredientType? GetByName(string name);
        void Add(IngredientType ingredient);
        void Delete(string name);
    }
}
