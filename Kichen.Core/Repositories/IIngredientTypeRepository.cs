using Kitchen.Core.Domain.Entities;
using Kitchen.Core.ValueObjects;

namespace Kitchen.Core.Repositories
{
    public interface IIngredientTypeRepository
    {
        IEnumerable<IngredientType> GetAll();
        IngredientType? GetByName(string name);
        void Add(IngredientType ingredient);
        void Delete(string name);
    }
}
