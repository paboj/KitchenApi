using Kitchen.Api.Commands;
using Kitchen.Api.Domain.Entities;

namespace Kitchen.Api.Services
{
    public interface ICatalogService
    {
        IEnumerable<IngredientType> GetAll();
        IngredientType? GetByName(string name);
        void Add(AddTypeCatalogCommand command);
        void Update(ModifyTypeCatalogCommand command);
        void Delete(string name);
    }
}
