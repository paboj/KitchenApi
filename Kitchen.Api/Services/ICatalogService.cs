using Kitchen.Api.Commands;
using Kitchen.Api.Domain.Entities;

namespace Kitchen.Api.Services
{
    public interface ICatalogService
    {
        IEnumerable<IngredientDefinition> GetAll();
        IngredientDefinition? GetByName(string name);
        void Add(AddToCatalogCommand command);
        bool Update(ModifyInCatalogCommand command);
        bool Delete(string name);
    }
}
