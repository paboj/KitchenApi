using Kitchen.Application.Commands;
using Kitchen.Core.Domain.Entities;

namespace Kitchen.Application.Services
{
    public interface ICatalogService
    {
        IEnumerable<ProductDefinition> GetAll();
        ProductDefinition? GetByName(string name);
        void Add(AddProductDefinitionCommand command);
        void Update(ModifyProductDefinitionCommand command);
        void Delete(string name);
    }
}
