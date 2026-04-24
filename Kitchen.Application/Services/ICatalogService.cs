using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;

namespace Kitchen.Application.Services
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
